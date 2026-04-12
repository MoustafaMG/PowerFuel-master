using System.Text.Json;

namespace PowerFuel.API.Services.FitnessCoach;

public interface IFitnessCoachClient
{
    Task<StartSessionResponseDto> StartSessionAsync(StartSessionRequestDto request, CancellationToken cancellationToken = default);

    Task<AnalyzeFrameResponseDto> AnalyzeFrameAsync(AnalyzeFrameRequestDto request, CancellationToken cancellationToken = default);

    Task<SessionSummaryResponseDto> EndSessionAsync(EndSessionRequestDto request, CancellationToken cancellationToken = default);

    Task<SessionSummaryResponseDto> GetSessionSummaryAsync(string sessionId, CancellationToken cancellationToken = default);
}

public sealed record StartSessionRequestDto(string Language = "en", string Level = "beginner");

public sealed record StartSessionResponseDto(string SessionId, string WsUrl);

public sealed record AnalyzeFrameRequestDto(string SessionId, FrameInputDto Frame);

public sealed class FrameInputDto
{
    public string? Exercise { get; set; }
    public Dictionary<string, double>? Angles { get; set; }
    public double Timestamp { get; set; }
    public int? FrameId { get; set; }
    public string? ImageB64 { get; set; }
}

public sealed class AnalyzeFrameResponseDto
{
    public string Feedback { get; set; } = string.Empty;
    public double Score { get; set; }
    public List<string> Issues { get; set; } = [];
    public int RepCount { get; set; }
    public string Exercise { get; set; } = string.Empty;
    public bool Paused { get; set; }
    public bool Speak { get; set; }
    public string Priority { get; set; } = "low";
    public string Lang { get; set; } = "en";
    public JsonElement? Debug { get; set; }
}

public sealed record EndSessionRequestDto(string SessionId);

public sealed record RepSummaryDto(int RepIndex, double Score, List<string> Issues);

public sealed record SessionSummaryResponseDto(
    string SessionId,
    string? Exercise,
    int Reps,
    double? AvgRepScore,
    double? BestRepScore,
    double? WorstRepScore,
    string? MostFrequentMistake,
    double ActiveTimeS,
    double IdleTimeS,
    List<RepSummaryDto> RepSummaries,
    Dictionary<string, int> IssuesTally);
