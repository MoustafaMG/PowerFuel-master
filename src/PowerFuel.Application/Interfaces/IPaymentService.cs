using PowerFuel.Application.DTOs.Payments;

namespace PowerFuel.Application.Interfaces;

public interface IPaymentService
{
    Task<CreatePaymentIntentResponse> CreateOrGetPaymentIntentAsync(int orderId, Guid userId, CancellationToken cancellationToken = default);
    Task HandleStripeWebhookAsync(string json, string stripeSignatureHeader, CancellationToken cancellationToken = default);
}

