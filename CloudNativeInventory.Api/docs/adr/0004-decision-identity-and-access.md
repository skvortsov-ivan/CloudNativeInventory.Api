### Architecture Decision Record: Identity and Access Management

### Context
The application needs secure access to Azure resources such as Key Vault without exposing credentials in code or configuration files.

### Decision
I chose **Managed Identity** combined with **Role-Based Access Control (RBAC)** for authentication and authorization.

**Motivation:**
- Managed Identity eliminates the need for hardcoded credentials.
- RBAC ensures least-privilege access to resources like Key Vault and ACR.
- Integration with Azure Container Apps allows automatic token handling.

### Consequences

**Positive consequences:**
- No secrets stored in code or configuration.
- Centralized access management through Azure RBAC.
- Simplified authentication for services within Azure.

**Negative consequences:**
- Requires propagation time after role assignments.
- Limited visibility into token lifecycle for debugging.

Owner: Ivan  
Date: 2026-05-19
