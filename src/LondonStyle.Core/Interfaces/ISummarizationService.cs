using LondonStyle.Core.Models;

namespace LondonStyle.Core.Interfaces;

/// <summary>
/// Service for making text more skimmable
/// </summary>
public interface ISummarizationService
{
    /// <summary>
    /// Makes text more skimmable by adding structure (bullets, headings, etc.)
    /// </summary>
    Task<string> MakeSkimmableAsync(SummarizationRequest request, CancellationToken cancellationToken = default);
}
