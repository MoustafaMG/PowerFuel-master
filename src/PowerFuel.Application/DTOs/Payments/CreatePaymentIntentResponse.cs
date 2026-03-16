namespace PowerFuel.Application.DTOs.Payments;

public record CreatePaymentIntentResponse(
    int OrderId,
    string PaymentIntentId,
    string ClientSecret,
    long Amount,
    string Currency,
    string Status
);

