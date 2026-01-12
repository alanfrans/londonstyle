# Deployment Checklist

Use this checklist when deploying the London Style AI Function App to production.

## Pre-Deployment

### Azure Resources Setup

- [ ] **Azure Subscription**
  - [ ] Access to Azure subscription confirmed
  - [ ] Appropriate permissions (Contributor or Owner role)
  
- [ ] **Azure OpenAI Service**
  - [ ] Azure OpenAI resource created
  - [ ] Model deployment created (GPT-4 or GPT-3.5-turbo)
  - [ ] Deployment name noted
  - [ ] API key retrieved
  - [ ] Endpoint URL noted
  - [ ] Regional deployment matches data residency requirements

- [ ] **Resource Planning**
  - [ ] Resource group name decided
  - [ ] Function app name decided (globally unique)
  - [ ] Storage account name decided (globally unique)
  - [ ] Region selected (e.g., eastus, westus2)

### Security & Compliance

- [ ] **Data Residency**
  - [ ] Azure OpenAI deployed in required region
  - [ ] Confirm data residency compliance
  
- [ ] **Secrets Management**
  - [ ] Azure Key Vault created (optional but recommended)
  - [ ] Plan for secret rotation
  
- [ ] **Authentication**
  - [ ] Decision made: Function Key vs Azure AD
  - [ ] SSO/Azure AD configuration planned (if applicable)

### Code & Configuration

- [ ] **Code Review**
  - [ ] All code committed to repository
  - [ ] No sensitive data in code
  - [ ] .gitignore properly configured
  
- [ ] **Configuration Files**
  - [ ] local.settings.json.template verified
  - [ ] requirements.txt reviewed
  - [ ] host.json configuration appropriate

## Deployment

### Option 1: Using deploy.sh Script

- [ ] Azure CLI installed
- [ ] Logged into Azure: `az login`
- [ ] Run deployment script: `./deploy.sh`
- [ ] Provide Azure OpenAI credentials when prompted
- [ ] Note the Function App URL from output
- [ ] Retrieve function keys

### Option 2: Manual Deployment

- [ ] Create resource group
  ```bash
  az group create --name <rg-name> --location <region>
  ```
  
- [ ] Create storage account
  ```bash
  az storage account create --name <storage-name> --resource-group <rg-name> --location <region>
  ```
  
- [ ] Create function app
  ```bash
  az functionapp create --name <app-name> --resource-group <rg-name> --runtime python --runtime-version 3.9
  ```
  
- [ ] Configure application settings
  ```bash
  az functionapp config appsettings set --name <app-name> --resource-group <rg-name> --settings KEY=VALUE
  ```
  
- [ ] Deploy code
  ```bash
  func azure functionapp publish <app-name>
  ```

## Post-Deployment

### Verification

- [ ] **Health Check**
  - [ ] GET /api/health returns 200 OK
  - [ ] Response contains correct service name
  
- [ ] **Endpoint Testing**
  - [ ] Test /api/simplify with sample text
  - [ ] Test /api/adjust-tone with both tones
  - [ ] Test /api/make-skimmable with both formats
  - [ ] Test /api/detect-jargon with sample text
  - [ ] Verify response times < 2 seconds
  - [ ] Verify all responses include ai_revised flag
  
- [ ] **Error Handling**
  - [ ] Test with invalid JSON
  - [ ] Test with missing required fields
  - [ ] Test with invalid parameters
  - [ ] Verify appropriate error responses

### Monitoring Setup

- [ ] **Application Insights**
  - [ ] Application Insights enabled
  - [ ] Instrumentation key configured
  - [ ] Custom telemetry verified
  
- [ ] **Alerts Configuration**
  - [ ] Function failure alert > 5%
  - [ ] Response time alert > 3s
  - [ ] Error rate alert > 1%
  - [ ] Token usage alert > 80% quota
  
- [ ] **Dashboard**
  - [ ] Request volume chart
  - [ ] Response time chart
  - [ ] Success/failure rate chart
  - [ ] Cost tracking enabled

### Security Configuration

- [ ] **Network Security**
  - [ ] HTTPS-only enforced
  - [ ] Function keys secured
  - [ ] IP restrictions configured (if needed)
  
- [ ] **Authentication**
  - [ ] Azure AD authentication configured (if applicable)
  - [ ] CORS settings configured
  - [ ] API Management connected (if applicable)
  
- [ ] **Secrets Management**
  - [ ] API keys stored in Key Vault (recommended)
  - [ ] Managed Identity enabled
  - [ ] Key rotation schedule established

### Documentation & Training

- [ ] **Internal Documentation**
  - [ ] API endpoints documented for team
  - [ ] Function keys shared securely
  - [ ] Troubleshooting guide available
  - [ ] Support contact information provided
  
- [ ] **User Training**
  - [ ] Demo conducted for stakeholders
  - [ ] Usage guidelines provided
  - [ ] Best practices shared
  - [ ] Feedback mechanism established

### Compliance & Governance

- [ ] **Data Privacy**
  - [ ] Zero-retention policy verified
  - [ ] Data residency confirmed
  - [ ] Privacy headers in responses confirmed
  
- [ ] **Audit Trail**
  - [ ] Logging enabled
  - [ ] Audit logs configured
  - [ ] Log retention policy set
  
- [ ] **Compliance Documentation**
  - [ ] Security assessment completed
  - [ ] Privacy impact assessment (if required)
  - [ ] Compliance checklist reviewed

## Production Readiness

### Performance

- [ ] **Load Testing**
  - [ ] Conducted load testing
  - [ ] Verified auto-scaling works
  - [ ] Confirmed performance under load
  
- [ ] **Optimization**
  - [ ] Token limits appropriate
  - [ ] Timeout settings optimized
  - [ ] Cold start time acceptable

### Cost Management

- [ ] **Budget**
  - [ ] Monthly budget established
  - [ ] Cost alerts configured
  - [ ] Usage monitoring enabled
  
- [ ] **Optimization**
  - [ ] Appropriate pricing tier selected
  - [ ] Token usage monitored
  - [ ] Unnecessary resources cleaned up

### Backup & DR

- [ ] **Backup Strategy**
  - [ ] Infrastructure as Code backed up
  - [ ] Configuration in version control
  - [ ] Deployment scripts tested
  
- [ ] **Disaster Recovery**
  - [ ] Recovery procedure documented
  - [ ] RPO/RTO defined
  - [ ] Failover plan (if multi-region)

## Go-Live

- [ ] **Final Checks**
  - [ ] All tests passed
  - [ ] Monitoring working
  - [ ] Documentation complete
  - [ ] Team trained
  
- [ ] **Communication**
  - [ ] Stakeholders notified
  - [ ] Go-live announcement prepared
  - [ ] Support plan in place
  
- [ ] **Monitoring**
  - [ ] Real-time monitoring active
  - [ ] On-call schedule established
  - [ ] Incident response plan ready

## Post-Go-Live (First Week)

- [ ] **Day 1 Review**
  - [ ] Check error rates
  - [ ] Review performance metrics
  - [ ] Address any immediate issues
  
- [ ] **Week 1 Review**
  - [ ] Analyze usage patterns
  - [ ] Review user feedback
  - [ ] Identify optimization opportunities
  - [ ] Adjust monitoring thresholds if needed

## Maintenance Schedule

### Daily
- [ ] Check error logs
- [ ] Monitor performance metrics
- [ ] Review any alerts

### Weekly
- [ ] Review cost reports
- [ ] Analyze usage trends
- [ ] Check for updates to dependencies

### Monthly
- [ ] Security review
- [ ] Performance optimization review
- [ ] User feedback analysis
- [ ] Documentation updates

### Quarterly
- [ ] Disaster recovery test
- [ ] Security audit
- [ ] Capacity planning review
- [ ] Cost optimization review

---

## Notes

**Deployment Date**: _________________

**Deployed By**: _________________

**Production URL**: _________________

**Emergency Contact**: _________________

**Issues Encountered**:
- 
- 

**Lessons Learned**:
- 
- 

---

**Sign-off**:

Technical Lead: _________________ Date: _______

Product Owner: _________________ Date: _______

Security Officer: _______________ Date: _______
