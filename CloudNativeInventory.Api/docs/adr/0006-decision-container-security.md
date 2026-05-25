### Architecture Decision Record: Container Security and Build Strategy

### Context
The project must ensure container security and minimize attack surface during build and runtime.

### Decision
I implemented a **multi-stage Docker build** and configured the container to run **rootless**.

**Motivation:**
- Multi-stage builds reduce image size by excluding SDK and build dependencies from the final runtime image.
- Running as a non-root user limits potential privilege escalation.
- This approach aligns with best practices for secure containerization.

### Consequences

**Positive consequences:**
- Smaller, more secure runtime image.
- Reduced attack surface through non-root execution.
- Faster deployments due to optimized image size.

**Negative consequences:**
- Requires additional Dockerfile configuration.
- Slight complexity when debugging permission-related issues.

Owner: Ivan  
Date: 2026-05-19
