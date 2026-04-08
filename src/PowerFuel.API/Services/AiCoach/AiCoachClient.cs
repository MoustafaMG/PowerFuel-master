using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PowerFuel.API.Services.AiCoach;

public sealed class AiCoachClient(HttpClient httpClient) : IAiCoachClient
{
    public async Task<JsonElement> ProcessExerciseAsync(string exercise, IFormFile file, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(exercise))
            throw new ArgumentException("Exercise is required.", nameof(exercise));

        if (file is null || file.Length <= 0)
            throw new ArgumentException("A non-empty image file is required.", nameof(file));

        using var form = new MultipartFormDataContent();

        // Match FastAPI signature: exercise: Form(...), file: UploadFile = File(...)
        form.Add(new StringContent(exercise), "exercise");

        await using var fileStream = file.OpenReadStream();
        using var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType ?? "application/octet-stream");
        form.Add(fileContent, "file", file.FileName);

        using var response = await httpClient.PostAsync("/process_exercise", form, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"AI service returned {(int)response.StatusCode} {response.ReasonPhrase}. Body: {payload}",
                null,
                response.StatusCode);
        }

        using var doc = JsonDocument.Parse(payload);
        return doc.RootElement.Clone();
    }
}

