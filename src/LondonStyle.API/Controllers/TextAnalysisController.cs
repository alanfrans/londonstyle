using LondonStyle.Core.Interfaces;
using LondonStyle.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace LondonStyle.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TextAnalysisController : ControllerBase
{
    private readonly ITextAnalysisService _textAnalysisService;
    private readonly ILogger<TextAnalysisController> _logger;

    public TextAnalysisController(
        ITextAnalysisService textAnalysisService,
        ILogger<TextAnalysisController> logger)
    {
        _textAnalysisService = textAnalysisService;
        _logger = logger;
    }

    /// <summary>
    /// Analyzes text for readability and provides suggestions
    /// </summary>
    /// <param name="request">Text analysis request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Analysis results with suggestions</returns>
    [HttpPost("analyze")]
    [ProducesResponseType(typeof(TextAnalysisResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TextAnalysisResponse>> AnalyzeText(
        [FromBody] TextAnalysisRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return BadRequest("Text cannot be empty");
        }

        _logger.LogInformation("Analyzing text with {Length} characters", request.Text.Length);

        var result = await _textAnalysisService.AnalyzeTextAsync(request, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Simplifies a sentence to a target grade level
    /// </summary>
    /// <param name="request">Sentence to simplify with target grade level</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Simplified sentence</returns>
    [HttpPost("simplify")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<string>> SimplifySentence(
        [FromBody] SimplifySentenceRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Sentence))
        {
            return BadRequest("Sentence cannot be empty");
        }

        _logger.LogInformation("Simplifying sentence to grade level {GradeLevel}", request.TargetGradeLevel);

        var result = await _textAnalysisService.SimplifySentenceAsync(
            request.Sentence,
            request.TargetGradeLevel,
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Health check endpoint to verify the API is running
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult HealthCheck()
    {
        return Ok(new { Status = "Healthy", Service = "LondonStyle Text Analysis API" });
    }
}
