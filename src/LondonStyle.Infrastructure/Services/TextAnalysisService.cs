using LondonStyle.Core.Interfaces;
using LondonStyle.Core.Models;

namespace LondonStyle.Infrastructure.Services;

/// <summary>
/// Basic text analysis service implementation
/// This is a placeholder that can be enhanced with actual LLM integration
/// </summary>
public class TextAnalysisService : ITextAnalysisService
{
    private readonly ILlmProvider? _llmProvider;

    public TextAnalysisService(ILlmProvider? llmProvider = null)
    {
        _llmProvider = llmProvider;
    }

    public async Task<TextAnalysisResponse> AnalyzeTextAsync(TextAnalysisRequest request, CancellationToken cancellationToken = default)
    {
        var response = new TextAnalysisResponse
        {
            OriginalText = request.Text,
            OriginalGradeLevel = CalculateGradeLevel(request.Text),
            RevisedByAI = false
        };

        // Detect issues
        response.Issues = DetectIssues(request.Text);

        // If LLM provider is available and simplification is needed, use it
        if (_llmProvider != null && response.OriginalGradeLevel > request.TargetGradeLevel)
        {
            try
            {
                var prompt = $"Simplify the following text to a {request.TargetGradeLevel}th grade reading level. " +
                            $"Use active voice and plain language. Text: {request.Text}";
                
                response.SimplifiedText = await _llmProvider.GenerateResponseAsync(prompt, cancellationToken);
                response.SimplifiedGradeLevel = CalculateGradeLevel(response.SimplifiedText);
                response.RevisedByAI = true;
            }
            catch
            {
                // If LLM fails, continue without simplification
                response.SimplifiedText = null;
            }
        }

        return response;
    }

    public async Task<string> SimplifySentenceAsync(string sentence, int targetGradeLevel, CancellationToken cancellationToken = default)
    {
        if (_llmProvider == null)
        {
            return sentence; // Return original if no LLM available
        }

        var prompt = $"Simplify this sentence to a {targetGradeLevel}th grade reading level using active voice: {sentence}";
        return await _llmProvider.GenerateResponseAsync(prompt, cancellationToken);
    }

    private List<TextIssue> DetectIssues(string text)
    {
        var issues = new List<TextIssue>();

        // Detect passive voice (simple heuristic)
        var passiveIndicators = new[] { "was ", "were ", "been ", "being " };
        foreach (var indicator in passiveIndicators)
        {
            var index = text.IndexOf(indicator, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                issues.Add(new TextIssue
                {
                    Type = "PassiveVoice",
                    Description = "Consider using active voice",
                    Position = index,
                    Length = indicator.Length
                });
            }
        }

        // Detect long sentences (potential hard to read)
        var sentences = text.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        var position = 0;
        foreach (var sentence in sentences)
        {
            var wordCount = sentence.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            if (wordCount > 20)
            {
                issues.Add(new TextIssue
                {
                    Type = "HardToRead",
                    Description = $"Long sentence ({wordCount} words). Consider breaking it up.",
                    Position = position,
                    Length = sentence.Length
                });
            }
            position += sentence.Length + 1;
        }

        return issues;
    }

    private double CalculateGradeLevel(string text)
    {
        // Simple Flesch-Kincaid Grade Level approximation
        var sentences = text.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries).Length;
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var wordCount = words.Length;
        var syllableCount = words.Sum(CountSyllables);

        if (sentences == 0 || wordCount == 0) return 0;

        var gradeLevel = 0.39 * (wordCount / (double)sentences) + 11.8 * (syllableCount / (double)wordCount) - 15.59;
        return Math.Max(0, Math.Round(gradeLevel, 1));
    }

    private int CountSyllables(string word)
    {
        // Simple syllable counting heuristic
        word = word.ToLower();
        var vowels = new[] { 'a', 'e', 'i', 'o', 'u', 'y' };
        var syllables = 0;
        var previousWasVowel = false;

        foreach (var c in word)
        {
            var isVowel = vowels.Contains(c);
            if (isVowel && !previousWasVowel)
            {
                syllables++;
            }
            previousWasVowel = isVowel;
        }

        // Adjust for silent e
        if (word.EndsWith("e"))
        {
            syllables--;
        }

        return Math.Max(1, syllables);
    }
}
