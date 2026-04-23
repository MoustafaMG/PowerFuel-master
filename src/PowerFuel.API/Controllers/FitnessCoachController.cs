using System.Net;
using Microsoft.AspNetCore.Mvc;
using PowerFuel.API.Services.FitnessCoach;

namespace PowerFuel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FitnessCoachController(IFitnessCoachClient fitnessCoach) : ControllerBase
{
    [HttpPost("start-session")]
    public async Task<IActionResult> StartSession([FromBody] StartSessionRequestDto? request, CancellationToken cancellationToken)
    {
        try
        {
            request ??= new StartSessionRequestDto();
            var result = await fitnessCoach.StartSessionAsync(request, cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }
        catch (FitnessCoachApiException ex)
        {
            return StatusCode((int)ex.StatusCode, ex.ResponseBody);
        }
    }

    [HttpPost("analyze-frame")]
    public async Task<IActionResult> AnalyzeFrame([FromBody] AnalyzeFrameRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await fitnessCoach.AnalyzeFrameAsync(request, cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }
        catch (FitnessCoachApiException ex)
        {
            return StatusCode((int)ex.StatusCode, ex.ResponseBody);
        }
    }

    [HttpPost("end-session")]
    public async Task<IActionResult> EndSession([FromBody] EndSessionRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await fitnessCoach.EndSessionAsync(request, cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }
        catch (FitnessCoachApiException ex)
        {
            return StatusCode((int)ex.StatusCode, ex.ResponseBody);
        }
    }

    [HttpGet("session-summary/{sessionId}")]
    public async Task<IActionResult> SessionSummary(string sessionId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await fitnessCoach.GetSessionSummaryAsync(sessionId, cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }
        catch (FitnessCoachApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound(ex.ResponseBody);
        }
        catch (FitnessCoachApiException ex)
        {
            return StatusCode((int)ex.StatusCode, ex.ResponseBody);
        }
    }
}
