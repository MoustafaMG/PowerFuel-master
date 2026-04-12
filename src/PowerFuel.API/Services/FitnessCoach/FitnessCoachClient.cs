using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PowerFuel.API.Services.FitnessCoach;

public sealed class FitnessCoachApiException(HttpStatusCode statusCode, string responseBody) : Exception(
    $"Fitness coach API returned {(int)statusCode}: {responseBody}")
{
    public HttpStatusCode StatusCode { get; } = statusCode;

    public string ResponseBody { get; } = responseBody;
}

public sealed class FitnessCoachClient(HttpClient http) : IFitnessCoachClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public Task<StartSessionResponseDto> StartSessionAsync(StartSessionRequestDto request, CancellationToken cancellationToken = default) =>
        SendAsync<StartSessionResponseDto>(HttpMethod.Post, "start-session", request, cancellationToken);

    public Task<AnalyzeFrameResponseDto> AnalyzeFrameAsync(AnalyzeFrameRequestDto request, CancellationToken cancellationToken = default) =>
        SendAsync<AnalyzeFrameResponseDto>(HttpMethod.Post, "analyze-frame", request, cancellationToken);

    public Task<SessionSummaryResponseDto> EndSessionAsync(EndSessionRequestDto request, CancellationToken cancellationToken = default) =>
        SendAsync<SessionSummaryResponseDto>(HttpMethod.Post, "end-session", request, cancellationToken);

    public async Task<SessionSummaryResponseDto> GetSessionSummaryAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            throw new ArgumentException("Session id is required.", nameof(sessionId));

        using var request = new HttpRequestMessage(HttpMethod.Get, $"session-summary/{Uri.EscapeDataString(sessionId)}");
        using var response = await http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        return await ReadSuccessAsync<SessionSummaryResponseDto>(response, cancellationToken).ConfigureAwait(false);
    }

    private async Task<T> SendAsync<T>(HttpMethod method, string relativeUri, object? body, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, relativeUri);
        if (body is not null)
            request.Content = JsonContent.Create(body, options: JsonOptions);

        using var response = await http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        return await ReadSuccessAsync<T>(response, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<T> ReadSuccessAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken).ConfigureAwait(false);
            if (result is null)
                throw new InvalidOperationException("Fitness coach API returned an empty JSON body.");
            return result;
        }

        var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        throw new FitnessCoachApiException(response.StatusCode, body);
    }
}
