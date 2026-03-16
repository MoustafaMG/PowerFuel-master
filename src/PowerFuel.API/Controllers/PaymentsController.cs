using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PowerFuel.Application.Interfaces;

namespace PowerFuel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _payments;

    public PaymentsController(IPaymentService payments) => _payments = payments;

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Client calls this to get a Stripe client_secret, then confirms payment on the client.
    [Authorize]
    [HttpPost("orders/{orderId:int}/payment-intent")]
    public async Task<IActionResult> CreateOrGetPaymentIntent(int orderId, CancellationToken cancellationToken)
    {
        var result = await _payments.CreateOrGetPaymentIntentAsync(orderId, UserId, cancellationToken);
        return Ok(result);
    }

    // Stripe calls this. Must be anonymous and must read the raw body.
    [AllowAnonymous]
    [HttpPost("stripe/webhook")]
    public async Task<IActionResult> StripeWebhook(CancellationToken cancellationToken)
    {
        var stripeSignature = Request.Headers["Stripe-Signature"].ToString();
        using var reader = new StreamReader(Request.Body);
        var json = await reader.ReadToEndAsync(cancellationToken);

        await _payments.HandleStripeWebhookAsync(json, stripeSignature, cancellationToken);
        return Ok();
    }
}

