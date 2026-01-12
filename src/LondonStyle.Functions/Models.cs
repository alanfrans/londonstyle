namespace LondonStyle.Functions;

public sealed record TextRequest(string? Text);

public sealed record AdjustToneRequest(string? Text, string? Tone);

public sealed record MakeSkimmableRequest(string? Text, string? Format);

public sealed record AnalyzeRequest(string? Text, int? TargetGradeLevel, string? Tone);

public sealed record TextIssue(
    string Type,
    int Position,
    int Length,
    string Description,
    string? Suggestion
);

public sealed record AnalyzeResponse(
    double OriginalGradeLevel,
    double TargetGradeLevel,
    TextIssue[] Issues,
    bool AiRevised
);
