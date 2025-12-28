namespace LondonStyle.Core.Models;

/// <summary>
/// Represents a request for text analysis and simplification
/// </summary>
public class TextAnalysisRequest
{
    /// <summary>
    /// The input text to be analyzed
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Target tone for the output (e.g., "GovernmentProfessional", "CommunityFriendly")
    /// </summary>
    public string? Tone { get; set; }

    /// <summary>
    /// Target grade level for readability (default: 7-8)
    /// </summary>
    public int TargetGradeLevel { get; set; } = 7;
}
