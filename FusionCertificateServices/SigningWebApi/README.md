# SigningWebApi

A .NET 8 ASP.NET Core Web API for handling document signing and stamping operations.

## Overview

SigningWebApi provides REST endpoints for cryptographic signing operations and document stamping. It integrates with the `Signing` and `Stamping` libraries to deliver secure document handling capabilities.

## Features

- **Document Signing** - Cryptographic signing of documents using certificates
- **Document Stamping** - Apply timestamps and metadata to documents
- **Swagger/OpenAPI** - Interactive API documentation at `/swagger`
- **Self-Contained Deployment** - Runs independently without requiring .NET runtime installation

## Prerequisites

- **.NET 8 Runtime** (for framework-dependent deployment) or
- **Windows x64** (for self-contained deployment)

## Getting Started

### Build

```bash
dotnet build
```

### Run Locally

```bash
dotnet run
```

The API will be available at `https://localhost:5001` (or the configured port in `appsettings.json`)

### API Documentation

Once running, access the Swagger UI at:
```
https://localhost:5001/swagger
```

## Publishing

### Self-Contained Deployment (Recommended)

Creates a standalone executable that includes the .NET runtime:

```bash
dotnet publish -c Release
```

Output location: `bin/Release/net8.0-windows/win-x64/publish/`

The published folder contains `SigningWebApi.exe` which can be run on any Windows x64 machine without .NET installed.

### Framework-Dependent Deployment

Creates a smaller deployment that requires .NET 8 runtime:

```bash
dotnet publish -c Release -p:SelfContained=false
```

## Configuration

Configuration is managed through `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

For production deployments, create `appsettings.Production.json` with appropriate settings.

## Project Structure

- **SigningWebApi.csproj** - Project configuration
- **Properties/** - Launch settings and profiles
- **Controllers/** - API endpoints
- **appsettings.json** - Application configuration

## Dependencies

- **Swashbuckle.AspNetCore** (v6.6.2) - Swagger/OpenAPI support
- **Signing** - Document signing library
- **Stamping** - Document stamping library

## Development

### Running Tests

Run the test suite from the solution root:

```bash
dotnet test SigningWithCertsTests
```

### IDE Support

- **Visual Studio 2022** - Full support
- **Visual Studio Code** - Requires C# extension

## Deployment

### Windows Server Deployment

1. Publish the application
2. Copy the `publish` folder to target machine
3. Run `SigningWebApi.exe`
4. Configure firewall and proxy rules as needed

### Docker Deployment

A Dockerfile can be added for containerized deployments if needed.

## Troubleshooting

- **Port in use** - Modify the port in `appsettings.json` or use `--urls` parameter
- **Certificate issues** - Ensure required certificates are installed on the deployment machine
- **HTTPS errors** - Check SSL certificate configuration in `appsettings.json`

## License

See repository LICENSE file.

## Support

For issues or questions, refer to the project repository or documentation.
