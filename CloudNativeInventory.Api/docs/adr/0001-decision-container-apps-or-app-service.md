## Architecture Decision Record: Choice of Azure Compute

### Context
CloudNativeInventory is a student project where we will containerize an ASP NET core WEB API project with the help of Docker.

### Decision
I chose Azure Container Apps as the primary compute platform.

Motivation:
- Container Apps can be automatically adjusted to the traffic and supports **scale-to-zero**, which minimizes cost during idle periods. App service does not support this.
- Running a container eliminates the problem of keeping the project associated packages up to date.

### Consequences

**Positive consequences:**
- Lower operational cost thanks to **scale-to-zero**.
- Automatic handling of traffic spikes without manual scaling.

**Negative consequences:**
- Learning curve for Container Apps concepts and implementation.

Owner: Ivan  
Date: 2026-05-05
