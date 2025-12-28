namespace LondonStyle.Core.Models;

/// <summary>
/// Represents the result of text analysis
/// </summary>
public class TextAnalysisResponse
{
    /// <summary>
    /// The original input text
    /// </summary>
    public string OriginalText { get; set; } = string.Empty;

    /// <summary>
    /// The simplified/improved version of the text
    /// </summary>
    public string? SimplifiedText { get; set; }

    /// <summary>
    /// Grade level of the original text
    /// </summary>
    public double OriginalGradeLevel { get; set; }

    /// <summary>
    /// Grade level of the simplified text
    /// </summary>
    public double? SimplifiedGradeLevel { get; set; }

    /// <summary>
    /// Issues detected in the text (passive voice, complex sentences, jargon, etc.)
    /// </summary>
    public List<TextIssue> Issues { get; set; } = new();

    /// <summary>
    /// Indicates whether AI was used to modify the text
    /// </summary>
    public bool RevisedByAI { get; set; }
}

/// <summary>
/// Represents an issue detected in the text
/// </summary>
public class TextIssue
{
    /// <summary>
    /// Type of issue (e.g., "HardToRead", "PassiveVoice", "Jargon", "UndefinedAcronym")
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Description of the issue
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Position in the text where the issue occurs
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Length of the problematic text segment
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Suggested replacement or fix
    /// </summary>
    public string? Suggestion { get; set; }
}
