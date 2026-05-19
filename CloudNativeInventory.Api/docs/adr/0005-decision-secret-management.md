### Architecture Decision Record: Secret Management

### Context
The API requires secure storage and retrieval of sensitive values such as API keys.

### Decision
I chose **Azure Key Vault** for secret management.

**Motivation:**
- Key Vault provides centralized, encrypted storage for secrets.
- It integrates directly with Managed Identity, allowing secure retrieval without manual credentials.
- Using Key Vault aligns with Azure best practices for compliance and security.

### Consequences

**Positive consequences:**
- Secure and centralized secret storage.
- Automatic rotation and versioning of secrets.
- Seamless integration with Container Apps via Managed Identity.

**Negative consequences:**
- Requires RBAC configuration and propagation time.
- Slight delay in startup when fetching secrets.

Owner: Ivan  
Date: 2026-05-05
