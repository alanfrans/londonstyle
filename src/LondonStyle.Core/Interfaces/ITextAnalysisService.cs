using LondonStyle.Core.Models;

namespace LondonStyle.Core.Interfaces;

/// <summary>
/// Service for analyzing and simplifying text
/// </summary>
public interface ITextAnalysisService
{
    /// <summary>
    /// Analyzes text for readability and suggests improvements
    /// </summary>
    Task<TextAnalysisResponse> AnalyzeTextAsync(TextAnalysisRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Simplifies a given sentence to a lower grade level
    /// </summary>
    Task<string> SimplifySentenceAsync(string sentence, int targetGradeLevel, CancellationToken cancellationToken = default);
}
