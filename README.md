"# LondonStyle - AI-Assisted Government Writing Tool

LondonStyle is a .NET 8 C# application that provides county employees with an AI-assisted writing tool. It ensures public communications are **clear, accessible, and compliant with Plain Language standards** (typically 6th–8th grade reading level).

## Features

- **Text Analysis**: Analyzes text for readability using the Flesch-Kincaid Grade Level
- **Issue Detection**: Identifies passive voice, long sentences, jargon, and undefined acronyms
- **AI-Powered Simplification**: (Optional) Simplifies complex sentences using LLM integration
- **Tone Adjustment**: Supports "Government Professional" and "Community Friendly" tones
- **Plain Language Standards**: Targets 6th-8th grade reading level for accessibility

## Technology Stack

- **.NET 8**: Latest long-term support version of .NET
- **ASP.NET Core Web API**: RESTful API for text analysis
- **C# 12**: Modern C# language features
- **xUnit**: Unit testing framework
- **Swagger/OpenAPI**: API documentation and testing interface

## Project Structure

```
LondonStyle/
├── src/
│   ├── LondonStyle.API/          # Web API project with controllers and endpoints
│   ├── LondonStyle.Core/         # Domain models and interfaces
│   └── LondonStyle.Infrastructure/ # Service implementations
├── tests/
│   └── LondonStyle.Tests/        # Unit tests
└── LondonStyle.sln               # Solution file
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- Your favorite IDE (Visual Studio 2022, VS Code, or JetBrains Rider)

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/alanfrans/londonstyle.git
cd londonstyle
```

### 2. Build the Solution

```bash
dotnet build
```

### 3. Run Tests

```bash
dotnet test
```

### 4. Run the API

```bash
cd src/LondonStyle.API
dotnet run
```

The API will start on `https://localhost:5001` (HTTPS) and `http://localhost:5000` (HTTP).

### 5. Access Swagger UI

Open your browser and navigate to:
```
https://localhost:5001/swagger
```

## API Endpoints

### Health Check
```
GET /api/textanalysis/health
```
Verifies the API is running.

### Analyze Text
```
POST /api/textanalysis/analyze
Content-Type: application/json

{
  "text": "Your text here",
  "tone": "GovernmentProfessional",
  "targetGradeLevel": 7
}
```

Returns analysis including:
- Original and simplified text
- Grade level scores
- Detected issues (passive voice, long sentences, etc.)
- Whether AI was used for revision

### Simplify Sentence
```
POST /api/textanalysis/simplify
Content-Type: application/json

{
  "sentence": "Your sentence here",
  "targetGradeLevel": 7
}
```

Returns a simplified version of the sentence.

## Configuration

Edit `src/LondonStyle.API/appsettings.json` to configure:

```json
{
  "LondonStyle": {
    "DefaultTargetGradeLevel": 7,
    "MaxTextLength": 50000,
    "Features": {
      "AiSimplification": false,
      "AiSummarization": false,
      "JargonDetection": true
    }
  }
}
```

## Privacy & Security

- **Zero-Retention**: Text is processed ephemerally and not stored
- **Data Residency**: Designed to support on-premise LLM deployment
- **No Training Data**: Input text is never used to train models
- **Human-in-the-Loop**: AI suggestions are clearly marked for human review

## Development

### Adding a New Service

1. Define the interface in `LondonStyle.Core/Interfaces/`
2. Implement the service in `LondonStyle.Infrastructure/Services/`
3. Register the service in `LondonStyle.API/Program.cs`
4. Add unit tests in `LondonStyle.Tests/`

### Running in Development Mode

```bash
cd src/LondonStyle.API
dotnet watch run
```

This enables hot reload for faster development.

## Testing

Run all tests:
```bash
dotnet test
```

Run tests with detailed output:
```bash
dotnet test --verbosity normal
```

Run tests with coverage:
```bash
dotnet test /p:CollectCoverage=true
```

## Future Enhancements

- Integration with Azure OpenAI or local LLM (Llama 3)
- SSO/Active Directory authentication
- WCAG 2.1 AA compliance improvements
- Jargon detection service implementation
- Summarization/skimmability features
- Batch processing support

## License

See the LICENSE file for details.

## Contributing

Please read CONTRIBUTING.md for details on our code of conduct and the process for submitting pull requests.

## Support

For issues and questions, please open an issue on GitHub." 
