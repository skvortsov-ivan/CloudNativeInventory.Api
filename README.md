# Project Overview
CloudNativeInventory.Api is a student project where the goal is to learn how to containerize a .NET 9 Web application for deployment to Azure with the help of DevOps practices. The solution includes a multi‑stage Dockerfile that produces a minimal, rootless runtime image designed to reduce the potential attack surface. The application is prepared to run both locally and in the cloud without exposing secrets, relying on Azure Key Vault when deployed. The project also integrates a CI/CD pipeline that automatically builds, tests, packages, and deploys the application to Azure. As part of this workflow, container images are stored in Azure Container Registry and then deployed to Azure Container Apps.

## How to run
After cloning the repository, the application can be run locally as long as the machine has a recent .NET SDK installed. The only requirement is to navigate into the API project folder, which can be done directly from PowerShell:
```
cd CloudNativeInventory.Api
```
Nothing needs to be configured beforehand, since the project uses an in‑memory database and avoids any dependency on external services during development. Once inside the correct directory, the application can be started with a simple command:
```
dotnet run
```

With the application running, it becomes possible to check that the API is working by writing the following url into the browser:
http://localhost:8080/api/inventory/system/verify-integration

This serves as a way to confirm that the application behaves as expected before introducing containers or cloud infrastructure.

When the application has been verified in this simple local mode, the next step is to run it inside a container. This requires Docker to be installed locally, after which the same project folder or the one above can be used to build the container image. 

The image can be created directly from PowerShell by running:

```
docker build -t inventory-app .
```

Once the image has been built, the container can be started in a way that exposes the API on port 8080, making it possible to interact with the application exactly as before:

```
docker run -p 8080:8080 inventory-app
```
For convenience, the repository also includes a Docker Compose configuration that automates both the build and the startup of the container. This makes it possible to launch the entire application with a single command:

```
docker compose up -d
```
This command builds the image if necessary and starts the container in the background, again making the API available at http://localhost:8080/api/inventory/system/verify-integration

## CI/CD Pipeline
When the application runs successfully in a container, the next step is to let the CI/CD pipeline handle the same process automatically. The pipeline is triggered whenever changes are pushed to the repository, and its job is to build, test, and package the application without requiring any manual steps. It restores dependencies, compiles the project, runs the test suite, and produces a versioned Docker image based on the same Dockerfile used locally. Once the image has been created, the pipeline pushes it to Azure Container Registry, making it available for deployment.

## Deployment to Azure
Azure Container Apps handles the deployment by pulling the exact image produced by the pipeline, allowing the application to run in a fully managed environment without any manual server administration. When the container runs in Azure, the platform provides the application access to other Azure services. Azure Key Vault is one of these services which is used as a storage for the application's secrets, ensuring that no sensitive information is included in the repository and Docker image. 

## ADR
Every decision regarding the pipeline design, Azure services and secret management can be found inside of the docs/adr folder inside of the respository. Url: 
https://github.com/skvortsov-ivan/CloudNativeInventory.Api/tree/master/CloudNativeInventory.Api/docs/adr
