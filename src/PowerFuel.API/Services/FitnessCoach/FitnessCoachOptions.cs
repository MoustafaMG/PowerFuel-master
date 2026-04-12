namespace PowerFuel.API.Services.FitnessCoach;

public sealed class FitnessCoachOptions
{
    public const string SectionName = "FitnessCoach";

    /// <summary>Base URL of the fitness-coach FastAPI (e.g. http://localhost:8000).</summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>Value sent as the X-API-Key header on every request.</summary>
    public string ApiKey { get; set; } = string.Empty;
}
