using Azure;
using Azure.AI.OpenAI;

namespace LondonStyle.Functions;

internal static class OpenAIClientFactory
{
    public static OpenAIClient Create()
    {
        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
        var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");

        if (string.IsNullOrWhiteSpace(endpoint))
            throw new InvalidOperationException("Missing AZURE_OPENAI_ENDPOINT.");
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("Missing AZURE_OPENAI_API_KEY.");

        return new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
    }

    public static string GetDeploymentName()
    {
        var deployment = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME");
        if (string.IsNullOrWhiteSpace(deployment))
            throw new InvalidOperationException("Missing AZURE_OPENAI_DEPLOYMENT_NAME.");
        return deployment;
    }
}
