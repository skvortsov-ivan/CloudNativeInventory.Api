### Architecture Decision Record: Choice of Container Registry

### Context
CloudNativeInventory requires a secure and reliable place to store and manage container images used in the CI/CD pipeline.

### Decision
I chose **Azure Container Registry (ACR)** as the image registry.

**Motivation:**
- ACR integrates with **Azure Container Apps** and **GitHub Actions**, enabling automated image builds and deployments.
- It supports private repositories and role-based access control (RBAC), ensuring secure image storage.
- Using ACR avoids external dependencies and simplifies authentication through **Managed Identity**.

### Consequences

**Positive consequences:**
- Simplified CI/CD integration with Azure services.
- Secure image storage with RBAC and private access.
- No need for external Docker Hub credentials.

**Negative consequences:**
- Slightly higher cost compared to public registries.
- Requires initial setup of permissions and network rules.

Owner: Ivan  
Date: 2026-05-19
