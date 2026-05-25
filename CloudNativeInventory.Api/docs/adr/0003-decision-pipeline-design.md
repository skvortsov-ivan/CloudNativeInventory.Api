### Architecture Decision Record: CI/CD Pipeline Design

### Context
The project needs an automated workflow to build, test, and deploy the containerized API to Azure.

### Decision
I designed a **GitHub Actions pipeline** that builds, tests, tags, and pushes the Docker image to ACR, followed by deployment to Azure Container Apps.

**Motivation:**
- GitHub Actions provides a simple YAML-based configuration that integrates directly with Azure.
- The pipeline ensures consistent builds and traceability through commit-based image tags.
- Automating deployment reduces manual errors and supports continuous delivery.

### Consequences

**Positive consequences:**
- Fully automated build and deployment process.
- Traceable image versions through commit SHA tags.
- Consistent environment setup across all deployments.

**Negative consequences:**
- Requires maintenance of secrets and permissions for GitHub–Azure integration.
- Initial setup complexity for service connections.

Owner: Ivan  
Date: 2026-05-19
