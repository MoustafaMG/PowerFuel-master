using Microsoft.AspNetCore.Mvc;
using PowerFuel.API.Services.AiCoach;

namespace PowerFuel.API.Controllers;

[ApiController]
[Route("api/ai-coach")]
public class AiCoachController : ControllerBase
{
    private readonly IAiCoachClient _client;

    public AiCoachController(IAiCoachClient client) => _client = client;

    [HttpPost("process-exercise")]
    [RequestSizeLimit(25_000_000)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ProcessExercise(
        [FromForm] string exercise,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var result = await _client.ProcessExerciseAsync(exercise, file, cancellationToken);
        return Ok(result);
    }
}

