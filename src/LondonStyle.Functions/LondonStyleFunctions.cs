using System.Net;
using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace LondonStyle.Functions;

public sealed class LondonStyleFunctions
{
    private readonly ILogger _logger;

    public LondonStyleFunctions(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<LondonStyleFunctions>();
    }

    private static async Task<string> ChatAsync(
        OpenAIClient client,
        string deployment,
        string systemPrompt,
        string userPrompt,
        float temperature,
        int maxTokens)
    {
        var options = new ChatCompletionsOptions
        {
            Temperature = temperature,
            MaxTokens = maxTokens
        };

        options.Messages.Add(new ChatRequestSystemMessage(systemPrompt));
        options.Messages.Add(new ChatRequestUserMessage(userPrompt));

        options.DeploymentName = deployment;

        var result = await client.GetChatCompletionsAsync(options);
        return result.Value.Choices[0].Message.Content?.Trim() ?? string.Empty;
    }

    private static async Task<T?> ReadBodyAsync<T>(HttpRequestData req)
    {
        return await JsonSerializer.DeserializeAsync<T>(req.Body, HttpResponseHelpers.JsonOptions);
    }

    [Function("simplify")]
    public async Task<HttpResponseData> Simplify(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "simplify")] HttpRequestData req)
    {
        _logger.LogInformation("Simplify function processed a request.");

        TextRequest? payload;
        try
        {
            payload = await ReadBodyAsync<TextRequest>(req);
        }
        catch
        {
            return await HttpResponseHelpers.Json(req, HttpStatusCode.BadRequest, new { error = "Invalid JSON" });
        }

        if (string.IsNullOrWhiteSpace(payload?.Text))
            return await HttpResponseHelpers.Json(req, HttpStatusCode.BadRequest, new { error = "Missing required field: text" });

        try
        {
            var client = OpenAIClientFactory.Create();
            var deployment = OpenAIClientFactory.GetDeploymentName();

            var systemPrompt = @"You are a Plain Language expert helping government employees write clearly.
Your task is to simplify sentences to a 6th-8th grade reading level while:
1. Maintaining the original factual meaning
2. Converting passive voice to active voice
3. Using shorter, simpler words
4. Breaking long sentences into shorter ones if needed

Respond ONLY with the simplified text, no explanations.";

            var userPrompt = $"Simplify this sentence for plain language (6th-8th grade level): {payload.Text}";
            var simplified = await ChatAsync(client, deployment, systemPrompt, userPrompt, 0.3f, 500);

            return await HttpResponseHelpers.Json(req, HttpStatusCode.OK, new
            {
                original = payload.Text,
                simplified,
                ai_revised = true,
                grade_level = "6th-8th grade target"
            });
        }
        catch (RequestFailedException ex) when (ex.Status == 429)
        {
            _logger.LogError(ex, "Azure OpenAI rate limit exceeded in simplify function");
            return await HttpResponseHelpers.Json(req, (HttpStatusCode)429, new { error = "Rate limit exceeded. Please try again later." });
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Azure OpenAI timeout in simplify function");
            return await HttpResponseHelpers.Json(req, HttpStatusCode.GatewayTimeout, new { error = "AI service timeout. Please try again." });
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Azure OpenAI API error in simplify function");
            return await HttpResponseHelpers.Json(req, HttpStatusCode.BadGateway, new { error = "AI service error. Please try again." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in simplify function");
            return await HttpResponseHelpers.Json(req, HttpStatusCode.InternalServerError, new { error = "Internal server error" });
        }
    }

    [Function("adjust-tone")]
    public async Task<HttpResponseData> AdjustTone(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "adjust-tone")] HttpRequestData req)
    {
        _logger.LogInformation("Adjust tone function processed a request.");

        AdjustToneRequest? payload;
        try
        {
            payload = await ReadBodyAsync<AdjustToneRequest>(req);
        }
        catch
        {
            return await HttpResponseHelpers.Json(req, HttpStatusCode.BadRequest, new { error = "Invalid JSON" });
        }

        if (string.IsNullOrWhiteSpace(payload?.Text))
            return await HttpResponseHelpers.Json(req, HttpStatusCode.BadRequest, new { error = "Missing required field: text" });

        var tone = string.IsNullOrWhiteSpace(payload.Tone) ? "government_professional" : payload.Tone;

        if (tone is not ("government_professional" or "community_friendly"))
            return await HttpResponseHelpers.Json(req, HttpStatusCode.BadRequest, new { error = "Invalid tone. Must be \"government_professional\" or \"community_friendly\"" });

        try
        {
            var client = OpenAIClientFactory.Create();
            var deployment = OpenAIClientFactory.GetDeploymentName();

            var systemPrompt = tone == "government_professional"
                ? @"You are a government communications expert.
Adjust the text to a professional government tone while keeping it plain and clear.
- Use formal but accessible language
- Maintain official authority
- Keep vocabulary simple but professional
- Use terms like ""residents,"" ""services,"" ""department""

Respond ONLY with the adjusted text, no explanations."
                : @"You are a community engagement expert.
Adjust the text to a warm, community-friendly tone while keeping it clear.
- Use conversational, friendly language
- Replace formal terms with everyday words (e.g., ""procure"" -> ""get"", ""residents"" -> ""neighbors"")
- Keep the tone welcoming and accessible
- Maintain accuracy and respect

Respond ONLY with the adjusted text, no explanations.";

            var userPrompt = $"Adjust this text for {tone.Replace('_', ' ')} tone: {payload.Text}";
            var adjusted = await ChatAsync(client, deployment, systemPrompt, userPrompt, 0.5f, 500);

            return await HttpResponseHelpers.Json(req, HttpStatusCode.OK, new
            {
                original = payload.Text,
                adjusted,
                tone,
                ai_revised = true
            });
        }
        catch (RequestFailedException ex) when (ex.Status == 429)
        {
            _logger.LogError(ex, "Azure OpenAI rate limit exceeded in adjust_tone function");
            return await HttpResponseHelpers.Json(req, (HttpStatusCode)429, new { error = "Rate limit exceeded. Please try again later." });
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Azure OpenAI timeout in adjust_tone function");
            return await HttpResponseHelpers.Json(req, HttpStatusCode.GatewayTimeout, new { error = "AI service timeout. Please try again." });
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Azure OpenAI API error in adjust_tone function");
            return await HttpResponseHelpers.Json(req, HttpStatusCode.BadGateway, new { error = "AI service error. Please try again." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in adjust_tone function");
            return await HttpResponseHelpers.Json(req, HttpStatusCode.InternalServerError, new { error = "Internal server error" });
        }
    }

    [Function("make-skimmable")]
    public async Task<HttpResponseData> MakeSkimmable(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "make-skimmable")] HttpRequestData req)
    {
        _logger.LogInformation("Make skimmable function processed a request.");

        MakeSkimmableRequest? payload;
        try
        {
            payload = await ReadBodyAsync<MakeSkimmableRequest>(req);
        }
        catch
        {
            return await HttpResponseHelpers.Json(req, HttpStatusCode.BadRequest, new { error = "Invalid JSON" });
        }

        if (string.IsNullOrWhiteSpace(payload?.Text))
            return await HttpResponseHelpers.Json(req, HttpStatusCode.BadRequest, new { error = "Missing required field: text" });

        var format = string.IsNullOrWhiteSpace(payload.Format) ? "bullets" : payload.Format;

        if (format is not ("bullets" or "subheadings"))
            return await HttpResponseHelpers.Json(req, HttpStatusCode.BadRequest, new { error = "Invalid format. Must be \"bullets\" or \"subheadings\"" });

        try
        {
            var client = OpenAIClientFactory.Create();
            var deployment = OpenAIClientFactory.GetDeploymentName();

            var systemPrompt = format == "bullets"
                ? @"You are a document formatting expert for government communications.
Convert the given text into a clear, skimmable bulleted list.
- Extract key points and present them as bullet points
- Keep each bullet concise (1-2 lines)
- Maintain all important information
- Use parallel structure
- Optimize for mobile reading

Respond with the bulleted list only, no introductory text."
                : @"You are a document formatting expert for government communications.
Add descriptive subheadings to the given text to make it skimmable.
- Create clear, descriptive subheadings for each major section
- Keep the original text but organize it under appropriate headings
- Use meaningful headings that help readers scan quickly
- Format as: ## Heading followed by the relevant text

Respond with the formatted text only.";

            var userPrompt = $"Make this text skimmable using {format}: {payload.Text}";
            var skimmable = await ChatAsync(client, deployment, systemPrompt, userPrompt, 0.3f, 800);

            return await HttpResponseHelpers.Json(req, HttpStatusCode.OK, new
            {
                original = payload.Text,
                skimmable,
                format,
                ai_revised = true
            });
        }
        catch (RequestFailedException ex) when (ex.Status == 429)
        {
            _logger.LogError(ex, "Azure OpenAI rate limit exceeded in make_skimmable function");
            return await HttpResponseHelpers.Json(req, (HttpStatusCode)429, new { error = "Rate limit exceeded. Please try again later." });
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Azure OpenAI timeout in make_skimmable function");
            return await HttpResponseHelpers.Json(req, HttpStatusCode.GatewayTimeout, new { error = "AI service timeout. Please try again." });
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Azure OpenAI API error in make_skimmable function");
            return await HttpResponseHelpers.Json(req, HttpStatusCode.BadGateway, new { error = "AI service error. Please try again." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in make_skimmable function");
            return await HttpResponseHelpers.Json(req, HttpStatusCode.InternalServerError, new { error = "Internal server error" });
        }
    }

    [Function("detect-jargon")]
    public async Task<HttpResponseData> DetectJargon(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "detect-jargon")] HttpRequestData req)
    {
        _logger.LogInformation("Detect jargon function processed a request.");

        TextRequest? payload;
        try
        {
            payload = await ReadBodyAsync<TextRequest>(req);
        }
        catch
        {
            return await HttpResponseHelpers.Json(req, HttpStatusCode.BadRequest, new { error = "Invalid JSON" });
        }

        if (string.IsNullOrWhiteSpace(payload?.Text))
            return await HttpResponseHelpers.Json(req, HttpStatusCode.BadRequest, new { error = "Missing required field: text" });

        try
        {
            var client = OpenAIClientFactory.Create();
            var deployment = OpenAIClientFactory.GetDeploymentName();

            var systemPrompt = "You are a Plain Language expert specializing in government communications.\n" +
                               "Analyze the given text for:\n" +
                               "1. Undefined acronyms\n" +
                               "2. Government/technical jargon\n" +
                               "3. Complex terminology that might confuse the general public\n\n" +
                               "For each item found, provide:\n" +
                               "- The term or acronym\n" +
                               "- What type it is (acronym, jargon, technical_term)\n" +
                               "- A definition if it's an acronym\n" +
                               "- A plain language alternative\n\n" +
                               "Format your response as a JSON array with this structure:\n" +
                               "[\n" +
                               "  {\n" +
                               "    \"term\": \"the jargon or acronym\",\n" +
                               "    \"type\": \"acronym|jargon|technical_term\",\n" +
                               "    \"definition\": \"what the acronym stands for (if applicable)\",\n" +
                               "    \"plain_language\": \"simpler alternative\"\n" +
                               "  }\n" +
                               "]\n\n" +
                               "If no jargon is found, return an empty array: []\n" +
                               "Respond ONLY with the JSON array, no other text.";

            var userPrompt = $"Identify jargon and acronyms in this text: {payload.Text}";
            var raw = await ChatAsync(client, deployment, systemPrompt, userPrompt, 0.2f, 1000);

            object jargonFound;
            try
            {
                jargonFound = JsonSerializer.Deserialize<object>(raw, HttpResponseHelpers.JsonOptions) ?? Array.Empty<object>();
            }
            catch
            {
                _logger.LogWarning("Failed to parse jargon detection response. Response length: {Length} chars", raw.Length);
                jargonFound = Array.Empty<object>();
            }

            return await HttpResponseHelpers.Json(req, HttpStatusCode.OK, new
            {
                original = payload.Text,
                jargon_found = jargonFound,
                ai_revised = false
            });
        }
        catch (RequestFailedException ex) when (ex.Status == 429)
        {
            _logger.LogError(ex, "Azure OpenAI rate limit exceeded in detect_jargon function");
            return await HttpResponseHelpers.Json(req, (HttpStatusCode)429, new { error = "Rate limit exceeded. Please try again later." });
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Azure OpenAI timeout in detect_jargon function");
            return await HttpResponseHelpers.Json(req, HttpStatusCode.GatewayTimeout, new { error = "AI service timeout. Please try again." });
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Azure OpenAI API error in detect_jargon function");
            return await HttpResponseHelpers.Json(req, HttpStatusCode.BadGateway, new { error = "AI service error. Please try again." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in detect_jargon function");
            return await HttpResponseHelpers.Json(req, HttpStatusCode.InternalServerError, new { error = "Internal server error" });

        }
    }

    [Function("health")]
    public Task<HttpResponseData> Health(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequestData req)
    {
        _logger.LogInformation("Health check processed.");

        return HttpResponseHelpers.Json(req, HttpStatusCode.OK, new
        {
            status = "healthy",
            service = "londonstyle-ai-functions",
            version = "1.0.0"
        });
    }

    [Function("analyze")]
    public async Task<HttpResponseData> Analyze(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "textanalysis/analyze")] HttpRequestData req)
    {
        _logger.LogInformation("Analyze function processed a request.");

        AnalyzeRequest? payload;
        try
        {
            payload = await ReadBodyAsync<AnalyzeRequest>(req);
        }
        catch
        {
            return await HttpResponseHelpers.Json(req, HttpStatusCode.BadRequest, new { error = "Invalid JSON" });
        }

        if (string.IsNullOrWhiteSpace(payload?.Text))
            return await HttpResponseHelpers.Json(req, HttpStatusCode.BadRequest, new { error = "Missing required field: text" });

        var targetGradeLevel = payload.TargetGradeLevel ?? 8;

        try
        {
            var client = OpenAIClientFactory.Create();
            var deployment = OpenAIClientFactory.GetDeploymentName();

            var systemPrompt = @"You are a Plain Language analysis expert for government communications.
Analyze the given text and identify issues that affect readability.

For each issue found, provide:
- type: One of 'VeryHardToRead', 'HardToRead', 'ComplexWord', 'Jargon', 'PassiveVoice', 'LongSentence', 'UndefinedAcronym', 'GrammarIssue'
- position: The character index where the issue starts (0-based)
- length: The number of characters the issue spans
- description: A brief explanation of the issue
- suggestion: A plain language alternative or fix

Also estimate the current grade level (Flesch-Kincaid) of the text.

Format your response as JSON with this exact structure:
{
  ""gradeLevel"": 12.5,
  ""issues"": [
    {
      ""type"": ""ComplexWord"",
      ""position"": 10,
      ""length"": 8,
      ""description"": ""'Utilize' is unnecessarily complex"",
      ""suggestion"": ""use""
    }
  ]
}

Respond ONLY with valid JSON, no other text.";

            var userPrompt = $"Analyze this text for plain language issues (target: grade {targetGradeLevel}): {payload.Text}";
            var raw = await ChatAsync(client, deployment, systemPrompt, userPrompt, 0.2f, 2000);

            // Parse the AI response
            double gradeLevel = 10.0;
            var issues = new List<TextIssue>();

            try
            {
                using var doc = JsonDocument.Parse(raw);
                var root = doc.RootElement;

                if (root.TryGetProperty("gradeLevel", out var gradeProp))
                {
                    gradeLevel = gradeProp.GetDouble();
                }

                if (root.TryGetProperty("issues", out var issuesProp) && issuesProp.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in issuesProp.EnumerateArray())
                    {
                        var type = item.TryGetProperty("type", out var t) ? t.GetString() ?? "ComplexWord" : "ComplexWord";
                        var position = item.TryGetProperty("position", out var p) ? p.GetInt32() : 0;
                        var length = item.TryGetProperty("length", out var l) ? l.GetInt32() : 1;
                        var description = item.TryGetProperty("description", out var d) ? d.GetString() ?? "" : "";
                        var suggestion = item.TryGetProperty("suggestion", out var s) ? s.GetString() : null;

                        issues.Add(new TextIssue(type, position, length, description, suggestion));
                    }
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to parse analyze response. Response: {Response}", raw);
            }

            return await HttpResponseHelpers.Json(req, HttpStatusCode.OK, new
            {
                originalGradeLevel = gradeLevel,
                targetGradeLevel,
                issues = issues.ToArray(),
                aiRevised = false
            });
        }
        catch (RequestFailedException ex) when (ex.Status == 429)
        {
            _logger.LogError(ex, "Azure OpenAI rate limit exceeded in analyze function");
            return await HttpResponseHelpers.Json(req, (HttpStatusCode)429, new { error = "Rate limit exceeded. Please try again later." });
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Azure OpenAI timeout in analyze function");
            return await HttpResponseHelpers.Json(req, HttpStatusCode.GatewayTimeout, new { error = "AI service timeout. Please try again." });
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Azure OpenAI API error in analyze function");
            return await HttpResponseHelpers.Json(req, HttpStatusCode.BadGateway, new { error = "AI service error. Please try again." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in analyze function");
            return await HttpResponseHelpers.Json(req, HttpStatusCode.InternalServerError, new { error = "Internal server error" });
        }
    }
}
