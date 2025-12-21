# London Style - Azure AI Function App

An Azure Function App that provides AI-powered writing assistance for government communications using Azure AI Foundry. This service ensures public communications are clear, accessible, and compliant with Plain Language standards.

## Features

This Function App implements the following AI capabilities:

### 1. **Simplify** (`/api/simplify`)
- One-click simplification of hard-to-read sentences
- Targets 6th-8th grade reading level
- Prioritizes active voice over passive voice
- Maintains factual accuracy

### 2. **Tone Adjustment** (`/api/adjust-tone`)
- **Government Professional**: Formal but accessible language for official communications
- **Community Friendly**: Warm, conversational tone for public engagement
- Vocabulary adaptation while maintaining authority

### 3. **Make Skimmable** (`/api/make-skimmable`)
- Converts long paragraphs into bulleted lists
- Adds descriptive subheadings
- Optimized for mobile reading

### 4. **Jargon Detection** (`/api/detect-jargon`)
- Identifies undefined acronyms
- Detects government/technical jargon
- Suggests plain language alternatives

## Privacy & Security Features

✅ **Zero-Retention Policy**: Text is processed ephemerally and not stored  
✅ **Data Residency**: All processing occurs within configured Azure region  
✅ **AI Transparency**: All responses include `ai_revised` flag  
✅ **Privacy Headers**: Response includes no-cache and ephemeral processing headers  
✅ **No Training Data**: Input data is never used to train the LLM

## Prerequisites

- Azure subscription
- Azure AI Foundry / Azure OpenAI Service deployment
- Python 3.9 or higher
- Azure Functions Core Tools v4
- Azure CLI (for deployment)

## Local Development Setup

### 1. Install Dependencies

```bash
# Install Azure Functions Core Tools (if not already installed)
# macOS
brew tap azure/functions
brew install azure-functions-core-tools@4

# Windows (via Chocolatey)
choco install azure-functions-core-tools-4

# Linux (via npm)
npm i -g azure-functions-core-tools@4 --unsafe-perm true

# Install Python dependencies
pip install -r requirements.txt
```

### 2. Configure Environment Variables

Update `local.settings.json` with your Azure AI Foundry credentials:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "python",
    "AZURE_OPENAI_ENDPOINT": "https://your-resource-name.openai.azure.com/",
    "AZURE_OPENAI_API_KEY": "your-api-key-here",
    "AZURE_OPENAI_DEPLOYMENT_NAME": "your-deployment-name",
    "AZURE_OPENAI_API_VERSION": "2024-02-15-preview"
  }
}
```

**Required Environment Variables:**
- `AZURE_OPENAI_ENDPOINT`: Your Azure OpenAI endpoint URL
- `AZURE_OPENAI_API_KEY`: API key for authentication
- `AZURE_OPENAI_DEPLOYMENT_NAME`: Name of your GPT model deployment
- `AZURE_OPENAI_API_VERSION`: API version (default: 2024-02-15-preview)

### 3. Run Locally

```bash
func start
```

The functions will be available at `http://localhost:7071/api/{function-name}`

## Deployment to Azure

### Option 1: Deploy via Azure CLI

```bash
# Login to Azure
az login

# Create a resource group
az group create --name londonstyle-rg --location eastus

# Create a storage account
az storage account create \
  --name londonstylestorage \
  --resource-group londonstyle-rg \
  --location eastus \
  --sku Standard_LRS

# Create a Function App (Linux)
az functionapp create \
  --resource-group londonstyle-rg \
  --consumption-plan-location eastus \
  --runtime python \
  --runtime-version 3.9 \
  --functions-version 4 \
  --name londonstyle-ai-functions \
  --storage-account londonstylestorage \
  --os-type Linux

# Configure application settings
az functionapp config appsettings set \
  --name londonstyle-ai-functions \
  --resource-group londonstyle-rg \
  --settings \
  AZURE_OPENAI_ENDPOINT="https://your-resource-name.openai.azure.com/" \
  AZURE_OPENAI_API_KEY="your-api-key" \
  AZURE_OPENAI_DEPLOYMENT_NAME="your-deployment-name" \
  AZURE_OPENAI_API_VERSION="2024-02-15-preview"

# Deploy the function app
func azure functionapp publish londonstyle-ai-functions
```

### Option 2: Deploy via VS Code

1. Install the [Azure Functions extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions)
2. Sign in to Azure
3. Right-click on the Function App project
4. Select "Deploy to Function App..."
5. Follow the prompts to create or select a Function App

### Option 3: CI/CD with GitHub Actions

Create `.github/workflows/deploy.yml`:

```yaml
name: Deploy to Azure Functions

on:
  push:
    branches:
      - main

env:
  AZURE_FUNCTIONAPP_NAME: londonstyle-ai-functions
  PYTHON_VERSION: '3.9'

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
    - name: 'Checkout GitHub Action'
      uses: actions/checkout@v3

    - name: Setup Python
      uses: actions/setup-python@v4
      with:
        python-version: ${{ env.PYTHON_VERSION }}

    - name: 'Install dependencies'
      run: |
        pip install -r requirements.txt

    - name: 'Run Azure Functions Action'
      uses: Azure/functions-action@v1
      with:
        app-name: ${{ env.AZURE_FUNCTIONAPP_NAME }}
        package: .
        publish-profile: ${{ secrets.AZURE_FUNCTIONAPP_PUBLISH_PROFILE }}
```

## API Documentation

### Base URL
- **Local**: `http://localhost:7071/api`
- **Azure**: `https://your-function-app.azurewebsites.net/api`

### Authentication
All endpoints (except health check) require function key authentication. Include the key in the request:
- Query parameter: `?code=your-function-key`
- Header: `x-functions-key: your-function-key`

---

### 1. Simplify Text

**Endpoint**: `POST /api/simplify`

**Request Body**:
```json
{
  "text": "The implementation of the aforementioned policy was conducted by the department."
}
```

**Response**:
```json
{
  "original": "The implementation of the aforementioned policy was conducted by the department.",
  "simplified": "The department put the policy into action.",
  "ai_revised": true,
  "grade_level": "6th-8th grade target"
}
```

---

### 2. Adjust Tone

**Endpoint**: `POST /api/adjust-tone`

**Request Body**:
```json
{
  "text": "Residents must procure permits prior to construction.",
  "tone": "community_friendly"
}
```

**Tone Options**:
- `government_professional`: Formal, official language
- `community_friendly`: Warm, conversational language

**Response**:
```json
{
  "original": "Residents must procure permits prior to construction.",
  "adjusted": "Neighbors need to get permits before they start building.",
  "tone": "community_friendly",
  "ai_revised": true
}
```

---

### 3. Make Skimmable

**Endpoint**: `POST /api/make-skimmable`

**Request Body**:
```json
{
  "text": "The county parks department offers various recreational programs including sports leagues, summer camps, and educational workshops. These programs are designed for all age groups and promote community engagement and healthy lifestyles. Registration is available online or in person at any community center.",
  "format": "bullets"
}
```

**Format Options**:
- `bullets`: Convert to bulleted list
- `subheadings`: Add descriptive section headings

**Response**:
```json
{
  "original": "The county parks department...",
  "skimmable": "• Sports leagues, summer camps, and educational workshops available\n• Programs for all ages\n• Promotes community engagement and healthy lifestyles\n• Register online or at any community center",
  "format": "bullets",
  "ai_revised": true
}
```

---

### 4. Detect Jargon

**Endpoint**: `POST /api/detect-jargon`

**Request Body**:
```json
{
  "text": "The PIO will disseminate the RFP to all stakeholders for procurement of new infrastructure."
}
```

**Response**:
```json
{
  "original": "The PIO will disseminate the RFP to all stakeholders for procurement of new infrastructure.",
  "jargon_found": [
    {
      "term": "PIO",
      "type": "acronym",
      "definition": "Public Information Officer",
      "plain_language": "communications staff member"
    },
    {
      "term": "RFP",
      "type": "acronym",
      "definition": "Request for Proposal",
      "plain_language": "request for bids"
    },
    {
      "term": "disseminate",
      "type": "jargon",
      "plain_language": "share"
    },
    {
      "term": "procurement",
      "type": "jargon",
      "plain_language": "buying"
    }
  ],
  "ai_revised": false
}
```

---

### 5. Health Check

**Endpoint**: `GET /api/health`

**Response**:
```json
{
  "status": "healthy",
  "service": "londonstyle-ai-functions",
  "version": "1.0.0"
}
```

## Testing

### Using cURL

```bash
# Simplify text
curl -X POST "http://localhost:7071/api/simplify" \
  -H "Content-Type: application/json" \
  -d '{"text": "The implementation was conducted by the department."}'

# Adjust tone
curl -X POST "http://localhost:7071/api/adjust-tone" \
  -H "Content-Type: application/json" \
  -d '{"text": "Residents must procure permits.", "tone": "community_friendly"}'

# Make skimmable
curl -X POST "http://localhost:7071/api/make-skimmable" \
  -H "Content-Type: application/json" \
  -d '{"text": "Long paragraph text...", "format": "bullets"}'

# Detect jargon
curl -X POST "http://localhost:7071/api/detect-jargon" \
  -H "Content-Type: application/json" \
  -d '{"text": "The PIO will disseminate the RFP."}'
```

### Using Python

```python
import requests

base_url = "http://localhost:7071/api"

# Simplify
response = requests.post(f"{base_url}/simplify", json={
    "text": "The implementation was conducted."
})
print(response.json())

# Adjust tone
response = requests.post(f"{base_url}/adjust-tone", json={
    "text": "Residents must procure permits.",
    "tone": "community_friendly"
})
print(response.json())
```

## Configuration for Government Use

### Data Residency

Ensure your Azure OpenAI resource is deployed in the appropriate region:

```bash
# Create Azure OpenAI resource in specific region
az cognitiveservices account create \
  --name londonstyle-openai \
  --resource-group londonstyle-rg \
  --kind OpenAI \
  --sku S0 \
  --location eastus  # or your required region
```

### Single Sign-On (SSO) Integration

To integrate with Active Directory/SSO:

1. Enable Azure AD authentication in the Function App
2. Configure in Azure Portal: Function App → Authentication → Add identity provider
3. Update `function_app.py` to validate tokens

### WCAG 2.1 AA Compliance

The API responses are designed to be consumed by accessible frontends. Ensure your client application:
- Provides proper ARIA labels
- Supports keyboard navigation
- Maintains sufficient color contrast
- Provides text alternatives for AI-revised content

## Performance & SLA

- **Target Latency**: < 2.0 seconds per request
- **Concurrent Requests**: Scales automatically with Azure Functions
- **Rate Limiting**: Configure in Azure API Management if needed

## Monitoring

View logs and metrics in Azure Portal:

```bash
# Stream logs
func azure functionapp logstream londonstyle-ai-functions

# Or in Azure Portal
# Function App → Monitoring → Log stream
```

Enable Application Insights for detailed telemetry:

```bash
az monitor app-insights component create \
  --app londonstyle-insights \
  --location eastus \
  --resource-group londonstyle-rg

# Link to Function App
az functionapp config appsettings set \
  --name londonstyle-ai-functions \
  --resource-group londonstyle-rg \
  --settings APPINSIGHTS_INSTRUMENTATIONKEY=your-key
```

## Troubleshooting

### Issue: "OpenAI API Error"
- Verify `AZURE_OPENAI_ENDPOINT` and `AZURE_OPENAI_API_KEY` are correct
- Ensure your Azure OpenAI deployment is active
- Check that `AZURE_OPENAI_DEPLOYMENT_NAME` matches your model deployment

### Issue: "Function timeout"
- Increase timeout in `host.json`: `"functionTimeout": "00:05:00"`
- Optimize prompt length
- Consider using async processing for large texts

### Issue: "Module not found"
- Run `pip install -r requirements.txt`
- Ensure Python version matches runtime (3.9+)

## Cost Optimization

- Use Azure Functions Consumption Plan for pay-per-use
- Set appropriate token limits in API calls
- Implement caching for repeated queries (if appropriate)
- Monitor usage via Azure Cost Management

## Security Best Practices

1. **Never commit** `local.settings.json` with real credentials
2. Use **Azure Key Vault** for production secrets
3. Enable **managed identity** for Azure service authentication
4. Implement **rate limiting** via Azure API Management
5. Regular **security audits** of dependencies
6. Enable **HTTPS only** in production

## Support & Contributing

For issues or questions, please refer to the main repository documentation.

## License

See LICENSE file in the repository root.

---

**Compliance Note**: This service is designed to meet government data privacy requirements including zero-retention policies and data residency controls. Always verify compliance with your organization's specific policies before deployment.
