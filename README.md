"# London Style - AI-Powered Plain Language Writing Assistant

An Azure Function App that provides AI-powered writing assistance for government communications, ensuring clarity, accessibility, and Plain Language compliance.

## 🚀 Quick Start

Get started in 5 minutes: [QUICKSTART.md](QUICKSTART.md)

## 📚 Documentation

- **[Quick Start Guide](QUICKSTART.md)** - Get up and running quickly
- **[Function App README](FUNCTION_APP_README.md)** - Complete API documentation, deployment guide
- **[Architecture Documentation](ARCHITECTURE.md)** - System design, components, and data flow
- **[Requirements](requirements.md)** - Original functional requirements

## 🎯 Features

### AI-Powered Writing Tools
- **Simplify**: Rewrite complex text to 6th-8th grade reading level
- **Tone Adjustment**: Switch between Government Professional and Community Friendly tones
- **Make Skimmable**: Convert long text to bullets or add subheadings
- **Jargon Detection**: Identify acronyms and jargon with plain language alternatives

### Government-Ready
- ✅ Zero-retention policy (ephemeral processing)
- ✅ Data residency compliance
- ✅ AI transparency markers
- ✅ Privacy-first architecture
- ✅ Azure AD/SSO ready

## 🏗️ Technology Stack

- **Runtime**: Azure Functions (Python 3.9+)
- **AI Service**: Azure AI Foundry / Azure OpenAI
- **Models**: GPT-4 or GPT-3.5-turbo
- **Hosting**: Serverless (Consumption Plan)
- **Monitoring**: Application Insights

## 📦 What's Included

```
londonstyle/
├── function_app.py              # Main Azure Functions code
├── host.json                    # Azure Functions configuration
├── requirements.txt             # Python dependencies
├── local.settings.json.template # Configuration template
├── deploy.sh                    # Deployment script
├── test_functions.py            # Test suite
├── .funcignore                  # Deployment exclusions
├── .gitignore                   # Git exclusions
├── README.md                    # This file
├── QUICKSTART.md               # Quick start guide
├── FUNCTION_APP_README.md      # Detailed documentation
├── ARCHITECTURE.md             # Architecture documentation
└── requirements.md             # Original requirements
```

## 🔧 Installation

```bash
# Clone the repository
git clone <repository-url>
cd londonstyle

# Install dependencies
pip install -r requirements.txt

# Configure settings
cp local.settings.json.template local.settings.json
# Edit local.settings.json with your Azure OpenAI credentials

# Run locally
func start
```

## 💻 Usage

### Local Testing

```bash
# Start the function app
func start

# Run test suite
python test_functions.py
```

### API Example

```bash
curl -X POST http://localhost:7071/api/simplify \
  -H "Content-Type: application/json" \
  -d '{"text": "The implementation was conducted by the department."}'
```

Response:
```json
{
  "original": "The implementation was conducted by the department.",
  "simplified": "The department did the work.",
  "ai_revised": true,
  "grade_level": "6th-8th grade target"
}
```

## 🚢 Deployment

### Quick Deploy

```bash
./deploy.sh
```

### Manual Deploy

See [FUNCTION_APP_README.md](FUNCTION_APP_README.md#deployment-to-azure) for detailed deployment instructions.

## 📊 Monitoring

After deployment, monitor your function app:

```bash
# Stream logs
func azure functionapp logstream <your-function-app-name>

# Or view in Azure Portal
# Navigate to: Function App → Monitoring → Log stream
```

## 🔐 Security

- All API endpoints require authentication (function key or Azure AD)
- Ephemeral processing - no data retention
- HTTPS-only communication
- Privacy headers in all responses
- Configurable data residency

## 📈 Performance

- **Target Latency**: < 2 seconds
- **Auto-scaling**: Handles burst traffic automatically
- **Cost**: ~$25/month for typical usage (100K requests)

## 🤝 Contributing

This is a government-focused project. Ensure all contributions:
- Maintain security and privacy standards
- Follow Plain Language principles
- Include appropriate documentation

## 📄 License

See LICENSE file in the repository root.

## 🆘 Support

- Check [QUICKSTART.md](QUICKSTART.md) for common issues
- Review [FUNCTION_APP_README.md](FUNCTION_APP_README.md) for detailed docs
- See [ARCHITECTURE.md](ARCHITECTURE.md) for system design

## 🎯 Success Metrics

The system aims to achieve:
- Average 7th-grade reading level across outputs
- < 2.0 second response time
- 80% adoption rate among Public Information Officers
- 40% reduction in Plain Language review time

---

**Built with ❤️ for government communications teams**" 
