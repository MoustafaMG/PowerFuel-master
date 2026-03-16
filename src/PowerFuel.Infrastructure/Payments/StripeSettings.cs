namespace PowerFuel.Infrastructure.Payments;

public sealed class StripeSettings
{
    public const string SectionName = "Stripe";

    public string SecretKey { get; init; } = default!;
    public string WebhookSecret { get; init; } = default!;
    public string Currency { get; init; } = "usd";
}

