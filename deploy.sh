#!/bin/bash

# Azure Function App Deployment Script
# This script deploys the London Style AI Function App to Azure

set -e

# Configuration
RESOURCE_GROUP="londonstyle-rg"
LOCATION="eastus"
STORAGE_ACCOUNT="londonstylestorage"
FUNCTION_APP_NAME="londonstyle-ai-functions"
PYTHON_VERSION="3.9"

# Color output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}London Style AI Function App Deployment${NC}"
echo "========================================="

# Check if Azure CLI is installed
if ! command -v az &> /dev/null; then
    echo -e "${RED}Error: Azure CLI is not installed.${NC}"
    echo "Please install it from: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli"
    exit 1
fi

# Check if logged in
echo -e "${YELLOW}Checking Azure login status...${NC}"
az account show &> /dev/null || {
    echo -e "${YELLOW}Not logged in. Initiating login...${NC}"
    az login
}

# Get current subscription
SUBSCRIPTION=$(az account show --query name -o tsv)
echo -e "${GREEN}Using subscription: ${SUBSCRIPTION}${NC}"

# Prompt for environment variables
echo ""
echo -e "${YELLOW}Please provide the following Azure OpenAI configuration:${NC}"
read -p "Azure OpenAI Endpoint (e.g., https://your-resource.openai.azure.com/): " AZURE_OPENAI_ENDPOINT
read -sp "Azure OpenAI API Key: " AZURE_OPENAI_API_KEY
echo ""
read -p "Azure OpenAI Deployment Name: " AZURE_OPENAI_DEPLOYMENT_NAME
read -p "Azure OpenAI API Version (default: 2024-02-15-preview): " AZURE_OPENAI_API_VERSION
AZURE_OPENAI_API_VERSION=${AZURE_OPENAI_API_VERSION:-2024-02-15-preview}

# Create resource group
echo ""
echo -e "${YELLOW}Creating resource group: ${RESOURCE_GROUP}${NC}"
az group create --name "$RESOURCE_GROUP" --location "$LOCATION" --output none
echo -e "${GREEN}✓ Resource group created${NC}"

# Create storage account
echo -e "${YELLOW}Creating storage account: ${STORAGE_ACCOUNT}${NC}"
az storage account create \
  --name "$STORAGE_ACCOUNT" \
  --resource-group "$RESOURCE_GROUP" \
  --location "$LOCATION" \
  --sku Standard_LRS \
  --output none
echo -e "${GREEN}✓ Storage account created${NC}"

# Create Function App
echo -e "${YELLOW}Creating Function App: ${FUNCTION_APP_NAME}${NC}"
az functionapp create \
  --resource-group "$RESOURCE_GROUP" \
  --consumption-plan-location "$LOCATION" \
  --runtime python \
  --runtime-version "$PYTHON_VERSION" \
  --functions-version 4 \
  --name "$FUNCTION_APP_NAME" \
  --storage-account "$STORAGE_ACCOUNT" \
  --os-type Linux \
  --output none
echo -e "${GREEN}✓ Function App created${NC}"

# Configure application settings
echo -e "${YELLOW}Configuring application settings...${NC}"
az functionapp config appsettings set \
  --name "$FUNCTION_APP_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --settings \
  AZURE_OPENAI_ENDPOINT="$AZURE_OPENAI_ENDPOINT" \
  AZURE_OPENAI_API_KEY="$AZURE_OPENAI_API_KEY" \
  AZURE_OPENAI_DEPLOYMENT_NAME="$AZURE_OPENAI_DEPLOYMENT_NAME" \
  AZURE_OPENAI_API_VERSION="$AZURE_OPENAI_API_VERSION" \
  --output none
echo -e "${GREEN}✓ Application settings configured${NC}"

# Deploy the function app
echo -e "${YELLOW}Deploying function code...${NC}"
func azure functionapp publish "$FUNCTION_APP_NAME" --python

# Get function app URL
FUNCTION_URL=$(az functionapp show --name "$FUNCTION_APP_NAME" --resource-group "$RESOURCE_GROUP" --query defaultHostName -o tsv)

echo ""
echo -e "${GREEN}=========================================${NC}"
echo -e "${GREEN}Deployment completed successfully!${NC}"
echo -e "${GREEN}=========================================${NC}"
echo ""
echo "Function App URL: https://${FUNCTION_URL}"
echo ""
echo "Available endpoints:"
echo "  - POST https://${FUNCTION_URL}/api/simplify"
echo "  - POST https://${FUNCTION_URL}/api/adjust-tone"
echo "  - POST https://${FUNCTION_URL}/api/make-skimmable"
echo "  - POST https://${FUNCTION_URL}/api/detect-jargon"
echo "  - GET  https://${FUNCTION_URL}/api/health"
echo ""
echo -e "${YELLOW}Note: You'll need to include the function key in requests.${NC}"
echo "Get the function key from Azure Portal or run:"
echo "  az functionapp keys list --name ${FUNCTION_APP_NAME} --resource-group ${RESOURCE_GROUP}"
echo ""
echo -e "${GREEN}Next steps:${NC}"
echo "1. Test the health endpoint: curl https://${FUNCTION_URL}/api/health"
echo "2. Configure SSO/Authentication if required"
echo "3. Set up monitoring and alerts"
echo "4. Review security settings"
