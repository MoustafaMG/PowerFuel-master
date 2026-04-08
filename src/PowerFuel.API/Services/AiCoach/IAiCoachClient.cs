using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace PowerFuel.API.Services.AiCoach;

public interface IAiCoachClient
{
    Task<JsonElement> ProcessExerciseAsync(string exercise, IFormFile file, CancellationToken cancellationToken);
}

