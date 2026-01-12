# Quick Start Guide

Get the London Style AI Function App running in 5 minutes.

## Prerequisites

- Python 3.9+ installed
- Azure account with Azure OpenAI access
- Azure Functions Core Tools v4

## Step 1: Clone and Setup

```bash
# Navigate to project directory
cd /path/to/londonstyle

# Create virtual environment
python -m venv .venv

# Activate virtual environment
# On macOS/Linux:
source .venv/bin/activate
# On Windows:
.venv\Scripts\activate

# Install dependencies
pip install -r requirements.txt
```

## Step 2: Configure Azure OpenAI

1. Log in to Azure Portal
2. Create or navigate to your Azure OpenAI resource
3. Go to "Keys and Endpoint"
4. Copy your endpoint and key
5. Go to "Model deployments" and note your deployment name

## Step 3: Configure Local Settings

```bash
# Copy the template
cp local.settings.json.template local.settings.json

# Edit local.settings.json with your values
```

Update these values:
```json
{
  "AZURE_OPENAI_ENDPOINT": "https://your-resource.openai.azure.com/",
  "AZURE_OPENAI_API_KEY": "your-actual-key-here",
  "AZURE_OPENAI_DEPLOYMENT_NAME": "gpt-35-turbo"
}
```

## Step 4: Run Locally

```bash
# Start the function app
func start
```

You should see:
```
Azure Functions Core Tools
...
Functions:
    adjust-tone: [POST] http://localhost:7071/api/adjust-tone
    detect-jargon: [POST] http://localhost:7071/api/detect-jargon
    health: [GET] http://localhost:7071/api/health
    make-skimmable: [POST] http://localhost:7071/api/make-skimmable
    simplify: [POST] http://localhost:7071/api/simplify
```

## Step 5: Test the API

### Option A: Using the Test Script

```bash
python test_functions.py
```

### Option B: Using cURL

```bash
# Test health check
curl http://localhost:7071/api/health

# Test simplify
curl -X POST http://localhost:7071/api/simplify \
  -H "Content-Type: application/json" \
  -d '{"text": "The implementation was conducted by the department."}'

# Test tone adjustment
curl -X POST http://localhost:7071/api/adjust-tone \
  -H "Content-Type: application/json" \
  -d '{"text": "Residents must procure permits.", "tone": "community_friendly"}'
```

### Option C: Using Python

```python
import requests

response = requests.post(
    "http://localhost:7071/api/simplify",
    json={"text": "The implementation was conducted."}
)
print(response.json())
```

## Step 6: Deploy to Azure (Optional)

```bash
# Make sure you're logged into Azure
az login

# Run the deployment script
./deploy.sh
```

The script will:
- Create resource group
- Create storage account
- Create function app
- Configure settings
- Deploy your code

## Verification

After deployment, test the live endpoint:

```bash
# Get your function URL from the deployment output
FUNCTION_URL="https://your-app.azurewebsites.net"

# Test health check
curl ${FUNCTION_URL}/api/health
```

## Troubleshooting

### Issue: "func: command not found"

Install Azure Functions Core Tools:
```bash
# macOS
brew tap azure/functions
brew install azure-functions-core-tools@4

# Windows (Chocolatey)
choco install azure-functions-core-tools-4

# Linux (npm)
npm install -g azure-functions-core-tools@4 --unsafe-perm true
```

### Issue: "OpenAI API Error"

1. Check your `local.settings.json` has the correct values
2. Verify your Azure OpenAI deployment is active
3. Ensure the deployment name matches exactly

### Issue: Python version mismatch

```bash
# Check your Python version
python --version  # Should be 3.9+

# If using pyenv, set the version
pyenv local 3.9.0
```

### Issue: Import errors

```bash
# Reinstall dependencies
pip install -r requirements.txt --force-reinstall
```

## Next Steps

1. **Read the full documentation**: See [FUNCTION_APP_README.md](FUNCTION_APP_README.md)
2. **Understand the architecture**: See [ARCHITECTURE.md](ARCHITECTURE.md)
3. **Configure for production**: Set up Azure AD, monitoring, alerts
4. **Integrate with your app**: Use the API endpoints in your application

## Common Configuration

### Using a Different Model

In `local.settings.json`:
```json
{
  "AZURE_OPENAI_DEPLOYMENT_NAME": "gpt-4"  // or gpt-35-turbo
}
```

### Changing the Region

When creating Azure OpenAI resource, choose your region:
```bash
az cognitiveservices account create \
  --name your-openai \
  --resource-group your-rg \
  --kind OpenAI \
  --sku S0 \
  --location eastus  // Change this
```

### Enable Detailed Logging

In `host.json`:
```json
{
  "logging": {
    "logLevel": {
      "default": "Information",
      "Function": "Information"
    }
  }
}
```

## Example Use Cases

### 1. Simplify a Press Release

```bash
curl -X POST http://localhost:7071/api/simplify \
  -H "Content-Type: application/json" \
  -d '{
    "text": "The county commissioners have authorized the allocation of resources for infrastructure improvements."
  }'
```

### 2. Make Public Notice Friendly

```bash
curl -X POST http://localhost:7071/api/adjust-tone \
  -H "Content-Type: application/json" \
  -d '{
    "text": "Residents shall submit applications prior to deadline.",
    "tone": "community_friendly"
  }'
```

### 3. Format Long Announcement

```bash
curl -X POST http://localhost:7071/api/make-skimmable \
  -H "Content-Type: application/json" \
  -d '{
    "text": "The parks department offers programs for all ages...",
    "format": "bullets"
  }'
```

## Support

For issues or questions:
1. Check [FUNCTION_APP_README.md](FUNCTION_APP_README.md) for detailed docs
2. Review [ARCHITECTURE.md](ARCHITECTURE.md) for system design
3. See the main [requirements.md](requirements.md) for feature specs

---

**Ready to go?** Start the function app with `func start` and visit http://localhost:7071/api/health
