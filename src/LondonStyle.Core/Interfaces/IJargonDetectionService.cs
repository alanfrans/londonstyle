namespace LondonStyle.Core.Interfaces;

/// <summary>
/// Service for detecting jargon and acronyms
/// </summary>
public interface IJargonDetectionService
{
    /// <summary>
    /// Detects jargon and undefined acronyms in text
    /// </summary>
    Task<List<string>> DetectJargonAsync(string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Suggests plain language alternatives for jargon
    /// </summary>
    Task<Dictionary<string, string>> GetJargonAlternativesAsync(List<string> jargonTerms, CancellationToken cancellationToken = default);
}
