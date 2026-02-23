# Signing Library

A .NET Standard 2.0 library providing Windows Certificate Store integration and REST API client functionality for document signing operations.

## Overview

The Signing library is the core component of the document signing solution. It provides:
- **Windows Certificate Store Access** - Load and manage X.509 certificates
- **REST API Client** - HTTP client for communicating with SigningWebApi
- **Certificate Management** - Retrieve certificates with private key support
- **Cross-Platform Compatibility** - Built on .NET Standard 2.0

## Features

- **Certificate Store Integration** - Direct access to Windows Certificate Store
- **Private Key Detection** - Identify certificates suitable for signing
- **REST Client** - Built-in HTTP client for SigningWebApi communication
- **Certificate Enumeration** - List and filter available certificates
- **JSON Support** - Newtonsoft.Json integration for data serialization

## Prerequisites

- **.NET Standard 2.0** compatible platform
- **.NET Framework 4.7.2+**, **.NET Core 2.0+**, or **.NET 8+**
- Windows Certificate Store (for certificate operations)

## Project Structure

- **WindowsCertificateStore.cs** - Certificate store access and management
- **SigningWebApiClient.cs** - HTTP client for REST API communication

## Dependencies

- **FusionP8Supporting.Common** (v1.0.0) - Common data models and utilities
- **Newtonsoft.Json** (v13.0.4) - JSON serialization and deserialization

## Classes

### WindowsCertificateStore

Provides static methods for accessing the Windows Certificate Store.

**Key Methods:**

```csharp
// Load a specific certificate by thumbprint
X509Certificate2 LoadCert(string thumbprint);

// Get all certificates from a store
List<X509Certificate2> GetCertificates(StoreName storeName, StoreLocation storeLocation);

// Get certificates with private keys only
List<X509Certificate2> GetCertificatesWithPrivateKey(StoreName storeName, StoreLocation storeLocation);

// Get certificate details
X509Certificate2 GetCertificateByThumbprint(string thumbprint);
```

### SigningWebApiClient

HTTP client for communicating with the SigningWebApi REST endpoints.

**Key Methods:**

```csharp
// Constructor
SigningWebApiClient(string baseUrl);

// Get all available certificates
Task<List<Certificate>> GetCertificatesAsync();

// Get certificates with private keys
Task<List<Certificate>> GetCertificatesWithPrivateKeysAsync();

// Sign a document
Task<byte[]> SignDocumentAsync(Stream fileStream, string fileName, string thumbprint);

// Generate hash with stamp
Task<byte[]> HashDocumentAsync(Stream fileStream, string fileName, string thumbprint);

// Sign PDF with visible stamp
Task<byte[]> SignPdfAsync(Stream fileStream, string fileName, string thumbprint, 
    StampConfiguration stampConfig, Stream stampImage = null);
```

## Usage Examples

### Example 1: Load a Certificate from Windows Store

```csharp
using Signing;
using System.Security.Cryptography.X509Certificates;

// Get certificate by thumbprint
var certificate = WindowsCertificateStore.LoadCert("1234567890ABCDEF");

if (certificate != null)
{
    Console.WriteLine($"Certificate: {certificate.Subject}");
    Console.WriteLine($"Valid From: {certificate.NotBefore}");
    Console.WriteLine($"Valid To: {certificate.NotAfter}");
    Console.WriteLine($"Has Private Key: {certificate.HasPrivateKey}");
}
```

### Example 2: List All Certificates with Private Keys

```csharp
using Signing;
using System.Security.Cryptography.X509Certificates;

// Get signing certificates
var signingCerts = WindowsCertificateStore.GetCertificatesWithPrivateKey(
    StoreName.My,
    StoreLocation.LocalMachine
);

foreach (var cert in signingCerts)
{
    Console.WriteLine($"{cert.Subject} - {cert.Thumbprint}");
}
```

### Example 3: Use REST API Client for Document Signing

```csharp
using Signing;

// Create client
var client = new SigningWebApiClient("https://signing-server:8443");

// Get available signing certificates
var certificates = await client.GetCertificatesWithPrivateKeysAsync();

// Sign a document
using (var fileStream = File.OpenRead("document.pdf"))
{
    byte[] signedDocument = await client.SignDocumentAsync(
        fileStream,
        "document.pdf",
        certificates[0].Thumbprint
    );
    
    File.WriteAllBytes("signed-document.pdf", signedDocument);
}
```

### Example 4: Sign PDF with Visible Stamp

```csharp
using Signing;

var client = new SigningWebApiClient("https://signing-server:8443");

var stampConfig = new StampConfiguration
{
    Text = "Approved",
    Position = "BottomRight",
    FontSize = 12,
    Rotation = 45
};

using (var fileStream = File.OpenRead("document.pdf"))
{
    byte[] signedPdf = await client.SignPdfAsync(
        fileStream,
        "document.pdf",
        "1234567890ABCDEF",
        stampConfig
    );
    
    File.WriteAllBytes("signed-document.pdf", signedPdf);
}
```

## Windows Certificate Store

### Certificate Location

This library accesses certificates from the Windows Certificate Store, specifically:
- **Store Name:** `My` (Personal)
- **Store Location:** `LocalMachine` or `CurrentUser`

### Installing Test Certificates

For development/testing, install a test certificate:

```powershell
# View certificates
Get-ChildItem Cert:\LocalMachine\My

# Import certificate
Import-PfxCertificate -FilePath "certificate.pfx" -CertStoreLocation "Cert:\LocalMachine\My" -Password (ConvertTo-SecureString "password" -AsPlainText -Force)
```

### Certificate Requirements

For signing operations:
1. Certificate must be installed in the Windows Certificate Store
2. Certificate must have a private key associated
3. Certificate should not be expired
4. User must have access permissions to the certificate

## Integration with Other Projects

- **SigningWebApi** - Uses this library as a dependency
- **SigningActivities** - References this library for Fusion P8 integration
- **SigningWithCertsTests** - Tests this library's functionality

## Common Patterns

### Pattern 1: Factory for Signed Documents

```csharp
public class DocumentSigner
{
    private readonly SigningWebApiClient _client;
    
    public DocumentSigner(string apiUrl)
    {
        _client = new SigningWebApiClient(apiUrl);
    }
    
    public async Task<byte[]> SignAsync(byte[] documentBytes, string thumbprint)
    {
        using (var ms = new MemoryStream(documentBytes))
        {
            return await _client.SignDocumentAsync(ms, "document.pdf", thumbprint);
        }
    }
}
```

### Pattern 2: Certificate Selection Dialog

```csharp
public class CertificateSelector
{
    public static X509Certificate2 SelectCertificate()
    {
        var certs = WindowsCertificateStore.GetCertificatesWithPrivateKey(
            StoreName.My,
            StoreLocation.LocalMachine
        );
        
        if (certs.Count == 0)
            throw new Exception("No certificates with private keys found");
        
        // Return first or implement UI selection
        return certs[0];
    }
}
```

## Error Handling

### Common Exceptions

| Exception | Cause | Solution |
|-----------|-------|----------|
| `CertificateNotFoundException` | Certificate not found in store | Verify thumbprint and certificate installation |
| `NoPrivateKeyException` | Certificate lacks private key | Use certificate with private key |
| `HttpRequestException` | Cannot connect to API | Verify API URL and network connectivity |
| `InvalidOperationException` | Signing operation failed | Check certificate validity and permissions |

## Performance Considerations

- **Certificate Loading** - First call loads from Windows Store; consider caching
- **API Calls** - Network-dependent; use async/await for UI responsiveness
- **Large Documents** - Stream processing to minimize memory usage

## Security Best Practices

1. **Never expose private keys** - This library works with Windows Certificate Store, keeping keys secure
2. **Use HTTPS** - Always communicate with SigningWebApi over HTTPS
3. **Certificate Validation** - Verify certificate chains and expiry before signing
4. **Access Control** - Restrict access to private key certificates
5. **Audit Logging** - Log all signing operations for compliance

## Troubleshooting

### Certificate Not Found

**Issue:** `WindowsCertificateStore.LoadCert()` returns null

**Solutions:**
1. Verify certificate is installed:
   ```powershell
   Get-ChildItem Cert:\LocalMachine\My | Where-Object Thumbprint -eq "YOUR_THUMBPRINT"
   ```
2. Check thumbprint format (40 hex characters)
3. Ensure sufficient permissions to access certificate

### Cannot Connect to API

**Issue:** `SigningWebApiClient` throws connection error

**Solutions:**
1. Verify SigningWebApi is running
2. Check URL is correct (include protocol: `https://`)
3. Verify firewall allows connections
4. Check SSL certificate is valid

### Missing Private Key

**Issue:** Certificate exists but has no private key

**Solutions:**
1. Reinstall certificate with private key
2. Import from `.pfx` file (contains private key)
3. Check `certificate.HasPrivateKey` property

## Related Documentation

- [Windows Certificate Store Documentation](https://docs.microsoft.com/en-us/windows/win32/seccrypto/certificate-store)
- [.NET Standard 2.0 Compatibility](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)
- [Newtonsoft.Json Documentation](https://www.newtonsoft.com/json)

## License

See repository LICENSE file.

## Support

For issues or questions, refer to the project repository or contact the development team.
