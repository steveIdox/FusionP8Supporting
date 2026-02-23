# Signing Solution

A comprehensive digital document signing solution for Fusion P8, providing certificate-based document signing with visible stamps, hash generation, and PDF document signing capabilities.

## Overview

The Signing solution provides:
- **Windows Certificate Store integration** - Load and manage X.509 certificates from the local machine certificate store
- **Document Signing API** - REST API for signing documents with digital signatures
- **PDF Stamping** - Add visible certificate information to signed PDF documents
- **Fusion P8 Custom Activity** - Integrates document signing into Fusion P8 lifecycle workflows
- **Certificate Management** - List and retrieve available certificates with private key support

## Architecture

The solution consists of three main components:

### 1. Core Library: `Signing` (.NET Standard 2.0)
The foundational library providing certificate management and API client functionality.

**Key Classes:**
- `WindowsCertificateStore` - Manages access to Windows certificate stores
- `SigningApiClient` - HTTP client for the Signing Web API

### 2. REST API: `SigningWebApi` (.NET 8.0 - Windows)
A self-contained, modern ASP.NET Core Web API that handles document signing and certificate operations.

**Key Components:**
- `SigningController` - REST endpoints for signing operations
- `SigningHelper` - Core signing logic using RSA cryptography
- `CertificateHelper` - Certificate retrieval and management

### 3. Custom Activity: `SigningActivities` (.NET Framework 4.7.2)
A Fusion P8 custom activity that integrates document signing into document lifecycle workflows.

**Components:**
- `SignDocumentActivity` - Lifecycle activity for signing documents
- `SignDocumentLifecycleEntry.xml` - Lifecycle configuration

## Target Frameworks

- **Signing** - .NET Standard 2.0 (compatible with .NET Framework 4.7.2+, .NET Core/.NET 5+)
- **SigningWebApi** - .NET 8.0 (Windows-specific)
- **SigningActivities** - .NET Framework 4.7.2
- **SigningWithCertsTests** - Unit tests for signing functionality

## Dependencies

### Signing (Core Library)
- Newtonsoft.Json (v13.0.4)
- Common project

### SigningWebApi
- Swashbuckle.AspNetCore (v6.6.2) - Swagger/OpenAPI support
- Signing, Common, Files, Logging, Stamping projects

### SigningActivities
- Fusion.Api & Fusion.BusinessApi (DLLs)
- Signing, Common, Files, Logging projects

## REST API Endpoints

### GET /api/Signing/certificates
Retrieve all available certificates from the Windows certificate store.

**Response:**
```json
[
  {
    "thumbprint": "ABC123...",
    "subject": "CN=John Doe, O=Company",
    "issuer": "CN=Root CA",
    "validFrom": "2024-01-01",
    "validTo": "2025-01-01",
    "hasPrivateKey": true
  }
]
```

### GET /api/Signing/certificates/private-keys
Retrieve certificates with private keys available (required for signing).

**Response:** Same format as above, filtered to include only certificates with private keys.

### POST /api/Signing/sign
Sign a document with a certificate.

**Request:**
- `file` (multipart/form-data) - Document file to sign
- `thumbprint` (string) - Certificate thumbprint

**Response:** Signed document as binary file

```bash
curl -X POST https://localhost:5001/api/Signing/sign \
  -F "file=@mydocument.pdf" \
  -F "thumbprint=ABC123..."
```

### POST /api/Signing/hash
Generate a hash of a document with visible certificate stamp.

**Request:**
- `file` (multipart/form-data) - Document file to hash
- `thumbprint` (string) - Certificate thumbprint

**Response:** Document with visible stamp information as binary file

### POST /api/Signing/signpdf
Sign a PDF document with visible certificate stamp.

**Request:**
- `file` (multipart/form-data) - PDF file to sign
- `thumbprint` (string) - Certificate thumbprint
- `stampConfiguration` (StampConfiguration object) - Stamp appearance settings
- `stampImage` (optional, multipart/form-data) - Custom stamp image

**Response:** Signed PDF with visible stamp as binary file

## Configuration

### SigningWebApi Configuration

**appsettings.json:**
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

### SigningActivities Configuration (Fusion P8)

Configure the Signing Web API URL in `app.config`:

```xml
<configuration>
  <appSettings>
    <add key="SigningWebApiUrl" value="https://signing-server:8443" />
  </appSettings>
</configuration>
```

Set activity parameters in Fusion P8:
- **SigningWebApiUrl** - Base URL of the Signing Web API (e.g., `https://localhost:5001`)

## Usage Examples

### Example 1: Using the Signing Web API Directly

```bash
# Get available certificates
curl https://signing-server:8443/api/Signing/certificates

# Sign a document
curl -X POST https://signing-server:8443/api/Signing/sign \
  -F "file=@document.pdf" \
  -F "thumbprint=1234567890ABCDEF" \
  -o signed-document.pdf
```

### Example 2: Using SigningApiClient in Code

```csharp
using idox.eim.fusionp8.supporting;

// Create client
var client = new SigningApiClient("https://signing-server:8443");

// Get certificates with private keys
var certificates = await client.GetCertificatesWithPrivateKeysAsync();

// Sign a document
using (var fileStream = File.OpenRead("document.pdf"))
{
    byte[] signedDocument = await client.SignDocumentAsync(
        fileStream,
        "document.pdf",
        "1234567890ABCDEF"
    );
    
    File.WriteAllBytes("signed-document.pdf", signedDocument);
}
```

### Example 3: Fusion P8 Custom Activity Integration

The `SignDocumentActivity` automatically:
1. Retrieves the document from the Fusion P8 lifecycle context
2. Fetches available certificates from the Signing Web API
3. Prompts the user to select a certificate
4. Signs the document using the selected certificate
5. Stores the signed document back in the repository

Configure in Fusion P8 Activity Designer:
- **Activity Type:** SignDocumentActivity
- **Parameters:**
  - `SigningWebApiUrl` = `https://signing-server:8443`

## Windows Certificate Store Access

The solution uses the Windows Certificate Store to retrieve signing certificates. Certificates must be:
1. Installed in the **LocalMachine\Personal** store
2. Have a private key associated (for signing operations)

### Loading a Specific Certificate

```csharp
using idox.eim.fusionp8.supporting;
using System.Security.Cryptography.X509Certificates;

// Load certificate by thumbprint
X509Certificate2 cert = WindowsCertificateStore.LoadCert("1234567890ABCDEF");

// Get all certificates
var allCerts = WindowsCertificateStore.GetCertificates(
    StoreName.My,
    StoreLocation.LocalMachine
);

// Get certificates with private keys only
var signingCerts = WindowsCertificateStore.GetCertificatesWithPrivateKey(
    StoreName.My,
    StoreLocation.LocalMachine
);
```

## Running the Signing Web API

### Development

```bash
cd SigningWebApi
dotnet run
```

Server will start on `https://localhost:5001` (dev) or `http://localhost:5000`

Access Swagger UI at: `https://localhost:5001/swagger`

### Production

```bash
cd SigningWebApi
dotnet publish -c Release
```

The application is configured to publish as a self-contained single file for Windows x64.

## Testing

Unit tests are located in the `SigningWithCertsTests` project.

```bash
dotnet test SigningWithCertsTests
```

### Test Configuration

Ensure test machine has:
- At least one certificate with private key in Windows certificate store
- Test certificates configured in `App.config`

```xml
<configuration>
  <appSettings>
    <add key="TestCertificateThumbprint" value="YOUR_TEST_CERT_THUMBPRINT" />
  </appSettings>
</configuration>
```

## Security Considerations

### Certificate Storage
- Certificates are stored in the secure Windows Certificate Store
- Private keys never leave the local machine
- Remote signing requires secure HTTPS communication

### API Security
- Use HTTPS in production (enforced by SigningWebApi configuration)
- Consider implementing authentication (API key, OAuth, etc.)
- Restrict access to `/api/Signing/certificates/private-keys` endpoint
- Audit all signing operations

### Certificate Installation
- Only install trusted certificates
- Ensure certificate chains are valid
- Regularly audit installed certificates

## Troubleshooting

### Certificate Not Found

**Error:** `Certificate with thumbprint ... not found`

**Solution:**
1. Verify certificate is installed in Windows Certificate Store
   ```powershell
   Get-ChildItem Cert:\LocalMachine\My
   ```
2. Check certificate thumbprint format (should be 40 hex characters)
3. Ensure certificate has private key (for signing operations)

### API Connection Failures

**Error:** `Connection refused` or `Unable to connect`

**Solution:**
1. Verify Signing Web API is running
2. Check firewall allows access on API port
3. Verify URL is correct and includes HTTPS
4. Check SSL certificate is trusted (in dev, may need to disable validation)

### Fusion P8 Activity Not Working

**Error:** Activity fails during execution

**Solution:**
1. Verify `SigningWebApiUrl` is configured correctly
2. Check Fusion P8 server can reach Signing Web API over network
3. Review Logging output for detailed error messages
4. Ensure certificate thumbprint is valid

### PDF Stamping Issues

**Error:** Signed PDF doesn't show visible stamp

**Solution:**
1. Verify `StampConfiguration` is properly configured
2. Check PDF is not read-only or encrypted
3. Ensure sufficient permissions to modify PDF
4. Review Stamping project configuration

## Related Projects

- **Common** - Shared data models (Certificate, Document, StampConfiguration)
- **Files** - File handling utilities
- **Logging** - Logging abstraction
- **Stamping** - PDF stamping and visualization
- **RabbitMQTests** - Messaging integration

## Architecture Diagram

```
┌─────────────────────────────────────┐
│       Fusion P8                     │
│  (Document Management System)       │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│   SigningActivities (.NET 4.7.2)   │
│  (Custom Lifecycle Activity)        │
└──────────────┬──────────────────────┘
               │
          HTTP/HTTPS
               │
               ▼
┌─────────────────────────────────────┐
│   SigningWebApi (.NET 8.0)          │
│  (REST API - Document Signing)      │
│                                     │
│  ├─ SigningController               │
│  ├─ SigningHelper                   │
│  └─ CertificateHelper               │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│   Signing Library (.NET Std 2.0)   │
│                                     │
│  ├─ WindowsCertificateStore         │
│  └─ SigningApiClient                │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  Windows Certificate Store          │
│  (X.509 Certificates with keys)     │
└─────────────────────────────────────┘
```

## License

See repository LICENSE file.

## Support

For issues or questions:
1. Check the Troubleshooting section
2. Review Logging output
3. Contact the development team
4. Create an issue in the repository
