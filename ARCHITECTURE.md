# Architecture Documentation

## Overview

The London Style AI Function App is a serverless Azure Functions application that provides AI-powered writing assistance for government communications. It integrates with Azure AI Foundry (Azure OpenAI Service) to deliver Plain Language compliance features.

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                        Client Applications                       │
│  (Web UI, Mobile Apps, Content Management Systems)              │
└────────────────────────────┬────────────────────────────────────┘
                             │ HTTPS
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                     Azure API Management                         │
│  - Authentication (Azure AD/SSO)                                 │
│  - Rate Limiting                                                 │
│  - Request Validation                                            │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                  Azure Function App (Python)                     │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │  HTTP Triggers                                           │   │
│  │  - /api/simplify         - Simplify hard-to-read text   │   │
│  │  - /api/adjust-tone      - Adjust tone/style           │   │
│  │  - /api/make-skimmable   - Create bullets/headings     │   │
│  │  - /api/detect-jargon    - Find acronyms/jargon        │   │
│  │  - /api/health           - Health check                │   │
│  └──────────────────────────┬───────────────────────────────┘   │
└─────────────────────────────┼───────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                 Azure AI Foundry / OpenAI Service                │
│  - GPT-4 / GPT-3.5 Models                                        │
│  - Prompt Engineering                                            │
│  - Regional Deployment (Data Residency)                          │
│  - Zero-Retention Configuration                                  │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                    Supporting Services                           │
│  - Azure Application Insights (Monitoring)                       │
│  - Azure Key Vault (Secret Management)                           │
│  - Azure Storage (Function App Runtime)                          │
└─────────────────────────────────────────────────────────────────┘
```

## Components

### 1. Azure Function App

**Runtime**: Python 3.9+  
**Hosting Plan**: Consumption Plan (serverless, pay-per-use)  
**Trigger Type**: HTTP

**Functions**:
- `simplify`: Rewrites text to 6th-8th grade reading level
- `adjust-tone`: Adjusts text tone (professional/friendly)
- `make-skimmable`: Converts text to bullets or adds subheadings
- `detect-jargon`: Identifies and suggests alternatives for jargon
- `health`: Health check endpoint

### 2. Azure AI Foundry / Azure OpenAI

**Model**: GPT-4 or GPT-3.5-turbo  
**Configuration**: 
- Regional deployment for data residency
- Zero-retention policy (no training on user data)
- Token limits configured per endpoint
- Temperature settings optimized per use case

### 3. Security Layer

**Authentication Options**:
- Azure AD / Single Sign-On
- Function Key Authentication (default)
- Managed Identity for service-to-service

**Data Privacy**:
- Ephemeral processing (no data persistence)
- Privacy headers in all responses
- HTTPS-only communication
- Regional data residency

### 4. Monitoring & Observability

**Azure Application Insights**:
- Request tracing
- Performance metrics
- Error logging
- Custom telemetry

**Key Metrics**:
- Response time (target: < 2.0s)
- Success rate
- Token usage
- Error rates

## Data Flow

### Simplify Endpoint Example

```
1. Client sends POST request with text
   ↓
2. Function validates request and extracts text
   ↓
3. Constructs prompt with Plain Language guidelines
   ↓
4. Sends to Azure OpenAI with optimal parameters
   ↓
5. Receives simplified text from AI
   ↓
6. Formats response with metadata (ai_revised flag)
   ↓
7. Adds privacy headers
   ↓
8. Returns JSON response to client
   ↓
9. No data retention - ephemeral processing
```

## Prompt Engineering Strategy

Each endpoint uses carefully crafted system prompts to ensure:

1. **Consistency**: Predictable output format
2. **Accuracy**: Maintains factual information
3. **Compliance**: Adheres to Plain Language standards
4. **Safety**: Avoids inappropriate content

### Example Prompt Structure

```python
system_prompt = """
Role definition: [Who the AI should act as]
Task: [What needs to be done]
Constraints: [Rules and limitations]
Output format: [How to format the response]
"""

user_prompt = "[Specific text to process]"
```

## Scalability

### Horizontal Scaling
- Azure Functions auto-scales based on demand
- No manual intervention required
- Handles burst traffic automatically

### Performance Optimization
- Token limits prevent excessive costs
- Response streaming for long content (future enhancement)
- Caching layer (optional, for repeated queries)

## Compliance & Governance

### Data Privacy Requirements

| Requirement | Implementation |
|-------------|----------------|
| Zero-Retention | No text stored; immediate processing and deletion |
| Data Residency | Azure OpenAI deployed in required region |
| Audit Trail | Application Insights logging (no PII) |
| GDPR Compliance | No data persistence, right to erasure N/A |
| Access Control | Azure AD integration, RBAC |

### Plain Language Standards

- Target reading level: 6th-8th grade
- Active voice preferred
- Short sentences
- Simple vocabulary
- Clear structure

## Deployment Topology

### Development Environment
```
Local Machine
├── Python Virtual Environment
├── Azure Functions Core Tools
├── Local Azure OpenAI connection
└── local.settings.json (config)
```

### Production Environment
```
Azure Cloud
├── Azure Function App (Consumption Plan)
├── Azure OpenAI Service
├── Application Insights
├── Azure Key Vault (secrets)
└── Azure API Management (optional)
```

## Error Handling Strategy

### Input Validation
- Missing required fields → 400 Bad Request
- Invalid JSON → 400 Bad Request
- Invalid parameters → 400 Bad Request

### Service Errors
- Azure OpenAI timeout → 504 Gateway Timeout
- Rate limit exceeded → 429 Too Many Requests
- Service unavailable → 503 Service Unavailable
- Internal errors → 500 Internal Server Error

### Retry Logic
- Client should implement exponential backoff
- Azure Functions has built-in retry for transient failures
- OpenAI SDK has automatic retry with exponential backoff

## Future Enhancements

### Phase 2 Features
- [ ] Batch processing for multiple texts
- [ ] WebSocket support for real-time streaming
- [ ] Custom vocabulary dictionary per organization
- [ ] Multi-language support
- [ ] Document-level analysis (vs. sentence-level)

### Phase 3 Features
- [ ] Machine learning model for reading level prediction
- [ ] Integration with content management systems
- [ ] Browser extension
- [ ] Mobile SDK

## Cost Optimization

### Token Usage
- Use shorter prompts
- Set appropriate max_tokens limits
- Cache common requests (if applicable)

### Function Execution
- Optimize cold start time
- Use appropriate timeout settings
- Monitor execution duration

### Estimated Costs (Monthly)

| Component | Usage | Estimated Cost |
|-----------|-------|----------------|
| Function App (Consumption) | 100K requests | $20 |
| Azure OpenAI (GPT-3.5) | 1M tokens | $2 |
| Application Insights | Standard tier | $2.30 |
| Storage | Minimal | $0.50 |
| **Total** | | **~$25/month** |

*Costs vary by region and actual usage*

## Troubleshooting Guide

### Common Issues

1. **"Module not found" error**
   - Solution: Run `pip install -r requirements.txt`

2. **"OpenAI API Error"**
   - Check environment variables
   - Verify API key and endpoint
   - Ensure deployment name is correct

3. **Slow response times**
   - Check Azure OpenAI quota
   - Optimize prompt length
   - Consider GPT-3.5 instead of GPT-4

4. **Function timeout**
   - Increase timeout in host.json
   - Reduce max_tokens parameter
   - Check Azure OpenAI performance

## Security Considerations

### Secrets Management
- Never commit `local.settings.json`
- Use Azure Key Vault for production
- Rotate API keys regularly

### Network Security
- Enable VNET integration (optional)
- Use private endpoints for Azure services
- Configure firewall rules

### API Security
- Implement rate limiting
- Use authentication (Azure AD)
- Validate all inputs
- Sanitize outputs

## Monitoring & Alerts

### Key Alerts to Configure

1. **Function Failures** > 5% → Alert
2. **Response Time** > 3s → Warning
3. **Token Usage** > 80% quota → Alert
4. **Error Rate** > 1% → Alert

### Dashboard Metrics

- Request volume over time
- Average response time
- Success/failure rates
- Token consumption
- Cost tracking

## Backup & Disaster Recovery

### Backup Strategy
- Infrastructure as Code (ARM templates)
- Configuration in version control
- No data to backup (ephemeral processing)

### Disaster Recovery
- Multi-region deployment (optional)
- Automatic failover via Traffic Manager
- RPO: N/A (no data persistence)
- RTO: < 1 hour (redeploy function)

## Compliance Checklist

- [x] Zero-retention policy implemented
- [x] Data residency configurable
- [x] AI transparency (ai_revised flag)
- [x] Privacy headers in responses
- [x] No training data usage
- [ ] SSO integration (to be configured)
- [ ] WCAG 2.1 AA compliance (frontend responsibility)
- [ ] Audit logging enabled

## References

- [Azure Functions Documentation](https://docs.microsoft.com/azure/azure-functions/)
- [Azure OpenAI Service](https://docs.microsoft.com/azure/cognitive-services/openai/)
- [Plain Language Guidelines](https://www.plainlanguage.gov/)
- [Federal Plain Language Guidelines](https://www.plainlanguage.gov/guidelines/)
