namespace LondonStyle.Functions;

public sealed record TextRequest(string? Text);

public sealed record AdjustToneRequest(string? Text, string? Tone);

public sealed record MakeSkimmableRequest(string? Text, string? Format);
