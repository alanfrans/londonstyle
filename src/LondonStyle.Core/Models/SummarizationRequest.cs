namespace LondonStyle.Core.Models;

/// <summary>
/// Request for making text more skimmable
/// </summary>
public class SummarizationRequest
{
    /// <summary>
    /// The input text to be made skimmable
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Whether to convert to bullet points
    /// </summary>
    public bool UseBulletPoints { get; set; } = true;

    /// <summary>
    /// Whether to add subheadings
    /// </summary>
    public bool AddSubheadings { get; set; } = true;
}
