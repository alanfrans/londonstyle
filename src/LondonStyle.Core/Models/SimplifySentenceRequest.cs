namespace LondonStyle.Core.Models;

/// <summary>
/// Request for simplifying a sentence
/// </summary>
public class SimplifySentenceRequest
{
    /// <summary>
    /// The sentence to simplify
    /// </summary>
    public string Sentence { get; set; } = string.Empty;

    /// <summary>
    /// Target grade level for the simplified sentence
    /// </summary>
    public int TargetGradeLevel { get; set; } = 7;
}
