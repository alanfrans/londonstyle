namespace LondonStyle.Core.Interfaces;

/// <summary>
/// Interface for LLM providers (Azure OpenAI, local models, etc.)
/// </summary>
public interface ILlmProvider
{
    /// <summary>
    /// Sends a prompt to the LLM and returns the response
    /// </summary>
    Task<string> GenerateResponseAsync(string prompt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the provider is available and configured
    /// </summary>
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
}
