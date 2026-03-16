using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PowerFuel.Application.DTOs.Payments;
using PowerFuel.Application.Interfaces;
using PowerFuel.Domain.Entities;
using PowerFuel.Infrastructure.Data;
using PowerFuel.Infrastructure.Payments;
using Stripe;

namespace PowerFuel.Infrastructure.Services;

public sealed class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _context;
    private readonly StripeSettings _stripe;
    private readonly PaymentIntentService _paymentIntents;

    public PaymentService(ApplicationDbContext context, IOptions<StripeSettings> stripeOptions)
    {
        _context = context;
        _stripe = stripeOptions.Value;
        StripeConfiguration.ApiKey = _stripe.SecretKey;
        _paymentIntents = new PaymentIntentService();
    }

    public async Task<CreatePaymentIntentResponse> CreateOrGetPaymentIntentAsync(int orderId, Guid userId, CancellationToken cancellationToken = default)
    {
        var order = await _context.Orders.AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId, cancellationToken);

        if (order is null)
            throw new InvalidOperationException("Order not found.");

        if (!string.Equals(order.Status, "Pending", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Order is not payable (status: {order.Status}).");

        var amount = ToMinorUnits(order.TotalAmount);
        var currency = _stripe.Currency.ToLowerInvariant();

        var existing = await _context.Payments
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync(p => p.OrderId == orderId && p.UserId == userId, cancellationToken);

        if (existing is not null && !IsTerminal(existing.Status))
        {
            return new CreatePaymentIntentResponse(
                orderId,
                existing.StripePaymentIntentId,
                existing.ClientSecret,
                existing.Amount,
                existing.Currency,
                existing.Status
            );
        }

        var metadata = new Dictionary<string, string>
        {
            ["orderId"] = orderId.ToString(),
            ["userId"] = userId.ToString()
        };

        var paymentIntent = await _paymentIntents.CreateAsync(new PaymentIntentCreateOptions
        {
            Amount = amount,
            Currency = currency,
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions { Enabled = true },
            Description = $"Order #{orderId}",
            Metadata = metadata
        }, requestOptions: new RequestOptions
        {
            IdempotencyKey = $"order:{orderId}:user:{userId}:amount:{amount}:{currency}"
        }, cancellationToken: cancellationToken);

        var payment = Payment.Create(
            orderId,
            userId,
            paymentIntent.Id,
            paymentIntent.ClientSecret,
            amount,
            currency,
            paymentIntent.Description,
            customerEmail: null
        );
        payment.UpdateStatus(paymentIntent.Status);

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreatePaymentIntentResponse(
            orderId,
            paymentIntent.Id,
            paymentIntent.ClientSecret,
            amount,
            currency,
            paymentIntent.Status
        );
    }

    public async Task HandleStripeWebhookAsync(string json, string stripeSignatureHeader, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_stripe.WebhookSecret))
            throw new InvalidOperationException("Stripe webhook secret is not configured.");

        var stripeEvent = EventUtility.ConstructEvent(json, stripeSignatureHeader, _stripe.WebhookSecret);

        if (stripeEvent.Type.StartsWith("payment_intent.", StringComparison.OrdinalIgnoreCase))
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent
                ?? throw new InvalidOperationException("Invalid Stripe payload.");

            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.StripePaymentIntentId == paymentIntent.Id, cancellationToken);

            if (payment is null)
                return; // Payment not created by our system (or already purged)

            payment.UpdateStatus(paymentIntent.Status);

            if (string.Equals(paymentIntent.Status, "succeeded", StringComparison.OrdinalIgnoreCase))
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == payment.OrderId, cancellationToken);
                if (order is not null && string.Equals(order.Status, "Pending", StringComparison.OrdinalIgnoreCase))
                    order.Status = "Confirmed";
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private static bool IsTerminal(string status) =>
        string.Equals(status, "succeeded", StringComparison.OrdinalIgnoreCase)
        || string.Equals(status, "canceled", StringComparison.OrdinalIgnoreCase);

    private static long ToMinorUnits(decimal amount)
    {
        // Assumes 2-decimal currencies (USD/EUR/etc). If you need JPY or multi-currency, we’ll extend this.
        return (long)Math.Round(amount * 100m, 0, MidpointRounding.AwayFromZero);
    }
}

