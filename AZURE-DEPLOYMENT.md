# Azure setup + deployment for `LondonStyle.Functions` (.NET 8 isolated)

This repo contains an **Azure Functions isolated worker** app (`src\LondonStyle.Functions`). This document describes how to provision the required Azure resources (including an AI model resource) and how to configure the Function App so it can run after deployment.

> Notes
> - The exact AI provider your code uses depends on which `ILlmProvider` implementation is wired up (OpenAI, Azure OpenAI, etc.). This guide covers **Azure OpenAI** because it’s a common Azure-native choice.
> - If your code uses OpenAI directly, you can adapt the “AI settings” section to use `OPENAI_API_KEY` and `OPENAI_BASE_URL` instead.

---

## 1) Prerequisites

- Azure subscription with permission to create:
  - Resource groups
  - Storage accounts
  - Function Apps
  - Application Insights / Log Analytics
  - Azure OpenAI (requires approval in many tenants)
- Tools:
  - Azure CLI (`az`)
  - Functions Core Tools v4
  - .NET SDK 8

Optional:
- VS Code + Azure Functions extension

---

## 2) Resources to create

You’ll typically create these resources:

### Core Function resources
1. **Resource Group**
2. **Storage Account** (required for Functions)
3. **Function App**
   - Recommended: **Flex Consumption** plan (FC1) when available
   - Alternative: Premium (EP) if required
4. **Application Insights** (recommended for logs/metrics)

### AI resources
5. **Azure OpenAI resource**
6. **Model deployment** inside Azure OpenAI (e.g., `gpt-4o-mini`, `gpt-4.1-mini`, etc.)

---

## 3) Provisioning via Azure Portal (quick path)

### A) Create Azure OpenAI + model deployment
1. Create an **Azure OpenAI** resource.
2. In Azure OpenAI Studio, create a **deployment** for your chosen model.
3. Record:
   - Azure OpenAI **endpoint** (e.g., `https://<name>.openai.azure.com/`)
   - Azure OpenAI **API key** (or plan to use Managed Identity)
   - **Deployment name** (e.g., `text-simplifier`)

### B) Create Function App
1. Create a **Storage Account**.
2. Create a **Function App**:
   - Runtime: **.NET / Isolated**
   - Version: **.NET 8**
   - Region: same as Storage + Azure OpenAI where possible
   - Enable **Application Insights**
3. (Recommended) Enable **Managed Identity** on the Function App:
   - Identity ? System assigned ? On

### C) Grant the Function access to Azure OpenAI
Choose one:

#### Option 1: API key (simplest)
- Store the Azure OpenAI key as an app setting (see Section 5).

#### Option 2: Managed Identity (recommended)
- In the Azure OpenAI resource ? Access control (IAM)
  - Assign role **Cognitive Services OpenAI User** to the Function App’s managed identity.
- Your code must use `DefaultAzureCredential` / Entra auth for Azure OpenAI.

---

## 4) Provisioning via Azure CLI (example)

> This is an example. Adjust names/region to your environment.

```bash
# Variables
RG="londonstyle-rg"
LOCATION="canadacentral"
STORAGE="londonstylestorage$RANDOM"   # must be globally unique
FUNCAPP="londonstyle-func-$RANDOM"    # must be globally unique

az group create -n $RG -l $LOCATION

# Storage account (required)
az storage account create \
  -g $RG -n $STORAGE \
  -l $LOCATION \
  --sku Standard_LRS \
  --kind StorageV2

# Create Function App (Consumption example; if you use Flex Consumption, create accordingly)
# NOTE: Flex Consumption has different required configuration and is recommended when available.
az functionapp create \
  -g $RG -n $FUNCAPP \
  --storage-account $STORAGE \
  --consumption-plan-location $LOCATION \
  --runtime dotnet-isolated \
  --functions-version 4

# Enable managed identity (recommended)
az functionapp identity assign -g $RG -n $FUNCAPP
```

If you want fully repeatable infra, prefer Bicep/Terraform (AVM modules) and include Application Insights + deployment storage configuration (especially for Flex Consumption).

---

## 5) Configure Function App settings

Your Functions app needs configuration via **App Settings** (Azure) or `local.settings.json` (local).

### A) Required platform settings
Azure Functions needs:
- `AzureWebJobsStorage` (set automatically when created via Portal/CLI)
- `FUNCTIONS_WORKER_RUNTIME=dotnet-isolated`

### B) AI settings (Azure OpenAI)
Add app settings (names are suggestions; match what your code expects):

**API-key auth**
- `AZURE_OPENAI_ENDPOINT` = `https://<your-resource-name>.openai.azure.com/`
- `AZURE_OPENAI_API_KEY` = `<key>`
- `AZURE_OPENAI_DEPLOYMENT` = `<deployment-name>`
- `AZURE_OPENAI_API_VERSION` = `2024-06-01` (example; use what your SDK supports)

**Managed Identity auth** (no key)
- `AZURE_OPENAI_ENDPOINT`
- `AZURE_OPENAI_DEPLOYMENT`
- `AZURE_OPENAI_API_VERSION`

> If your code uses different environment variable names, update these to match.

### C) Configure settings via CLI

```bash
az functionapp config appsettings set \
  -g $RG -n $FUNCAPP \
  --settings \
  FUNCTIONS_WORKER_RUNTIME=dotnet-isolated \
  AZURE_OPENAI_ENDPOINT="https://<name>.openai.azure.com/" \
  AZURE_OPENAI_API_KEY="<key>" \
  AZURE_OPENAI_DEPLOYMENT="<deployment>" \
  AZURE_OPENAI_API_VERSION="2024-06-01"
```

---

## 6) Local development

1. Create `local.settings.json` in `src\LondonStyle.Functions` (do not commit secrets).

Example:

```json
{
  "IsEncrypted": false,
  "Values": {
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",

    "AZURE_OPENAI_ENDPOINT": "https://<name>.openai.azure.com/",
    "AZURE_OPENAI_API_KEY": "<key>",
    "AZURE_OPENAI_DEPLOYMENT": "<deployment>",
    "AZURE_OPENAI_API_VERSION": "2024-06-01"
  }
}
```

2. Run:

```bash
cd src/LondonStyle.Functions
func start
```

---

## 7) Deploy function code

### Option A: Deploy with Functions Core Tools

```bash
cd src/LondonStyle.Functions
func azure functionapp publish <your-function-app-name>
```

### Option B: Deploy from GitHub (recommended)
- Use GitHub Actions with `azure/functions-action`.
- Store secrets in GitHub:
  - `AZURE_FUNCTIONAPP_PUBLISH_PROFILE` or use OIDC federation

---

## 8) Verify

- Check Function App logs:
  - Azure Portal ? Function App ? **Log stream**
  - Application Insights ? **Failures / Traces**
- Verify the AI calls:
  - If using API key: confirm settings are present
  - If using Managed Identity: confirm role assignment and SDK auth

---

## 9) Common pitfalls

- **CORS**: If you call the Function from a browser, configure CORS on the Function App.
- **Missing model deployment**: Azure OpenAI resource exists but no deployment name configured.
- **Wrong API version**: Must match the SDK you’re using.
- **Networking**: Private endpoints/VNET can block the Function from reaching Azure OpenAI unless configured.

---

## 10) Next step (recommended)

If you want this to be more “one command” reproducible, add an `infra` folder with Bicep (AVM modules) that provisions:
- Function App on Flex Consumption (FC1)
- Storage + deployment storage config
- Application Insights
- Azure OpenAI + deployment
- Role assignment for Managed Identity

---

## Quick walkthrough (Central US + Azure OpenAI + API key auth)

This is the simplest end-to-end path for your answers:
- AI provider: **Azure OpenAI**
- Region: **Central US**
- Auth: **API key** stored in Function App settings

### 1) Create the Resource Group

```bash
RG="londonstyle-rg"
LOCATION="centralus"
az group create -n $RG -l $LOCATION
```

### 2) Create Azure OpenAI + deploy a model

Create the Azure OpenAI resource in the Portal (often easiest due to access/approval), then in Azure OpenAI Studio:

1. Deploy a model (example: `gpt-4o-mini`)
2. Copy these values:
   - `AZURE_OPENAI_ENDPOINT` (example: `https://<name>.openai.azure.com/`)
   - `AZURE_OPENAI_API_KEY`
   - `AZURE_OPENAI_DEPLOYMENT` (the *deployment name* you chose)

### 3) Create Storage (required)

```bash
# Must be globally unique and 3-24 lowercase letters/numbers
STORAGE="londonstylefunc$RANDOM"

az storage account create \
  -g $RG -n $STORAGE \
  -l $LOCATION \
  --sku Standard_LRS \
  --kind StorageV2
```

### 4) Create Application Insights (recommended)

```bash
AI_NAME="londonstyle-ai-$RANDOM"

# Create a Log Analytics workspace (needed by workspace-based Application Insights)
LAW_NAME="londonstyle-law-$RANDOM"
az monitor log-analytics workspace create -g $RG -n $LAW_NAME -l $LOCATION

LAW_ID=$(az monitor log-analytics workspace show -g $RG -n $LAW_NAME --query id -o tsv)

az monitor.app-insights.component create \
  -g $RG -l $LOCATION -a $AI_NAME \
  --workspace $LAW_ID
```

### 5) Create the Function App (.NET 8 isolated)

```bash
FUNCAPP="londonstyle-func-$RANDOM"

az functionapp create \
  -g $RG -n $FUNCAPP \
  --storage-account $STORAGE \
  --consumption-plan-location $LOCATION \
  --runtime dotnet-isolated \
  --functions-version 4
```

> If your tenant supports **Flex Consumption**, prefer that plan for production. The creation steps differ (deployment storage config). This repo’s setup will still work on standard Consumption for initial testing.

### 6) Configure Function App settings for Azure OpenAI

```bash
# Fill these in from your Azure OpenAI resource / deployment
AZURE_OPENAI_ENDPOINT="https://<name>.openai.azure.com/"
AZURE_OPENAI_API_KEY="<key>"
AZURE_OPENAI_DEPLOYMENT="<deployment-name>"
AZURE_OPENAI_API_VERSION="2024-06-01"

az functionapp config appsettings set \
  -g $RG -n $FUNCAPP \
  --settings \
  FUNCTIONS_WORKER_RUNTIME=dotnet-isolated \
  AZURE_OPENAI_ENDPOINT="$AZURE_OPENAI_ENDPOINT" \
  AZURE_OPENAI_API_KEY="$AZURE_OPENAI_API_KEY" \
  AZURE_OPENAI_DEPLOYMENT="$AZURE_OPENAI_DEPLOYMENT" \
  AZURE_OPENAI_API_VERSION="$AZURE_OPENAI_API_VERSION"
```

### 7) Deploy the Function code

From your repo:

```bash
cd src/LondonStyle.Functions
func azure functionapp publish $FUNCAPP
```

### 8) Verify

- Portal ? Function App ? **Functions** (your triggers should appear)
- Portal ? Function App ? **Log stream**
- Portal ? Application Insights ? **Traces / Failures**

---

## App settings checklist (API key auth)

These should exist in the Function App:
- `AzureWebJobsStorage` (created automatically)
- `FUNCTIONS_WORKER_RUNTIME=dotnet-isolated`
- `AZURE_OPENAI_ENDPOINT`
- `AZURE_OPENAI_API_KEY`
- `AZURE_OPENAI_DEPLOYMENT`
- `AZURE_OPENAI_API_VERSION`
