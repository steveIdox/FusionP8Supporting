# SigningActivities

A .NET Framework 4.7.2 class library providing Fusion P8 custom lifecycle activities for document signing integration.

## Overview

SigningActivities is a Fusion P8 extension that integrates digital document signing capabilities into document lifecycle workflows. It enables automated or manual signing of documents as part of Fusion P8 document management processes, with integration to the SigningWebApi for remote signing operations.

## Features

- **Custom Lifecycle Activity** - Seamless integration with Fusion P8 lifecycle workflows
- **Certificate Selection** - User-friendly certificate selection during document signing
- **Remote Signing** - Communicates with SigningWebApi for signing operations
- **Document Processing** - Automatic retrieval and storage of documents in Fusion P8
- **Signature Application** - Apply digital signatures and visible stamps to documents
- **Error Handling** - Robust error handling and logging for troubleshooting

## Prerequisites

- **.NET Framework 4.7.2** (required for Fusion P8 compatibility)
- **Fusion P8** (2021+ recommended) with custom activities support
- **Fusion P8 SDK** - For development and deployment
- **SigningWebApi** - Running and accessible from Fusion P8 server
- **FusionP8Supporting.Common** (v1.0.0) - Common utilities

## Project Structure

- **SignDocumentActivity.cs** - Main lifecycle activity class
- **Properties/AssemblyInfo.cs** - Assembly metadata

## Dependencies

### NuGet Packages
- **FusionP8Supporting.Common** (v1.0.0) - Common data models
- **Newtonsoft.Json** (v13.0.4) - JSON serialization
- **Signing** (project reference) - Signing client library

### Fusion P8 References
- **Fusion.Api** - Fusion P8 API client
- **Fusion.BusinessApi** - Fusion P8 business logic API

## Classes

### SignDocumentActivity

Main lifecycle activity class that implements document signing functionality.

**Key Methods:**

```csharp
// Lifecycle activity execution
public void Execute(LifecycleRequest request);

// Certificate selection and retrieval
public Certificate SelectCertificate();

// Document signing
public void SignDocument(Document document, string certificateThumbprint);

// Error handling
public void HandleError(Exception ex);
```

## Installation

### 1. Compile the Project

```bash
dotnet build SigningActivities
```

Or in Visual Studio:
1. Open the solution
2. Right-click `SigningActivities` project
3. Select **Build**

### 2. Deploy to Fusion P8

Copy the compiled DLL to the Fusion P8 custom activities directory:

```
C:\Program Files\Fusion P8\CustomActivities\
```

### 3. Register in Fusion P8

1. Open Fusion P8 Administration Console
2. Navigate to **Lifecycle Management** → **Custom Activities**
3. Click **Register New Activity**
4. Select the compiled `SigningActivities.dll`
5. Configure the activity parameters

## Configuration

### Fusion P8 Activity Parameters

Configure these parameters when adding the activity to a lifecycle:

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `SigningWebApiUrl` | String | Yes | Base URL of SigningWebApi (e.g., `https://signing-server:8443`) |
| `StampConfiguration` | Object | No | Stamp appearance settings (position, font, rotation) |
| `AutoSign` | Boolean | No | Auto-sign without user interaction (requires pre-selected certificate) |
| `CertificateThumbprint` | String | No | Thumbprint of certificate to use (if `AutoSign` is true) |

### Example Lifecycle Configuration

```xml
<Activity Name="SignDocument" Type="SigningActivities.SignDocumentActivity">
  <Parameters>
    <Parameter Name="SigningWebApiUrl">https://signing-server:8443</Parameter>
    <Parameter Name="StampConfiguration">
      <Position>BottomRight</Position>
      <FontSize>12</FontSize>
      <Rotation>45</Rotation>
    </Parameter>
    <Parameter Name="AutoSign">false</Parameter>
  </Parameters>
</Activity>
```

## Workflow Integration

### Typical Workflow

1. **Document Submission** - Document enters a lifecycle with signing activity
2. **Activity Trigger** - Signing activity is triggered based on lifecycle state transition
3. **Certificate Selection** - User selects a signing certificate from available options
4. **Signing Request** - Document is sent to SigningWebApi via SigningApiClient
5. **Signature Application** - Document is signed with selected certificate and stamp
6. **Document Update** - Signed document is stored back in Fusion P8 repository
7. **Lifecycle Continuation** - Workflow continues to next state

### Example Lifecycle States

```
Draft
  ↓
Review
  ↓
[Sign Document Activity] ← User signs document here
  ↓
Signed
  ↓
Published
```

## Usage Examples

### Example 1: Automatic Signing with Pre-configured Certificate

Configure in Fusion P8 Activity Designer:
- `SigningWebApiUrl` = `https://signing-server:8443`
- `AutoSign` = `true`
- `CertificateThumbprint` = `1234567890ABCDEF`

The activity will automatically sign documents without user interaction.

### Example 2: Manual Certificate Selection

Configure in Fusion P8 Activity Designer:
- `SigningWebApiUrl` = `https://signing-server:8443`
- `AutoSign` = `false`

When the activity executes:
1. User is presented with available certificates
2. User selects a certificate
3. Document is signed with selected certificate
4. Signed document replaces the original

### Example 3: Custom Stamp Appearance

```xml
<StampConfiguration>
  <Position>TopRight</Position>
  <Text>Approved</Text>
  <FontSize>14</FontSize>
  <Rotation>45</Rotation>
  <Color>#FF0000</Color>
  <Opacity>80</Opacity>
</StampConfiguration>
```

## Integration with Other Components

- **SigningWebApi** - Performs actual signing operations
- **Signing Library** - Provides API client and certificate management
- **Stamping Library** - Applies visible stamps to signed documents
- **Common Library** - Shared data models (Certificate, Document, StampConfiguration)

## Error Handling

### Common Errors

| Error | Cause | Resolution |
|-------|-------|-----------|
| Connection Failed | Cannot reach SigningWebApi | Verify URL and network connectivity |
| Certificate Not Found | Specified certificate unavailable | Verify certificate is installed and accessible |
| Signing Failed | Signature operation error | Check certificate validity and permissions |
| Document Update Failed | Cannot write signed document to repository | Verify repository permissions |

### Logging

The activity logs detailed information for troubleshooting:

```
[2024-01-15 10:30:45] SigningActivity: Starting document signing
[2024-01-15 10:30:45] SigningActivity: Retrieved 3 available certificates
[2024-01-15 10:30:50] SigningActivity: User selected certificate: CN=John Doe
[2024-01-15 10:30:51] SigningActivity: Document signed successfully
[2024-01-15 10:30:52] SigningActivity: Signed document updated in repository
```

Access logs in Fusion P8 Activity Monitor or system event logs.

## Development

### Building from Source

```bash
cd SigningActivities
dotnet build -c Release
```

Output: `bin/Release/SigningActivities.dll`

### Debugging

1. Enable debugging in Visual Studio
2. Attach to Fusion P8 process
3. Set breakpoints in `SignDocumentActivity.cs`
4. Trigger activity from Fusion P8 workflow

### Extending the Activity

Create a derived class to customize behavior:

```csharp
public class CustomSigningActivity : SignDocumentActivity
{
    public override void Execute(LifecycleRequest request)
    {
        // Custom logic before signing
        PreProcessDocument(request);
        
        // Call base implementation
        base.Execute(request);
        
        // Custom logic after signing
        PostProcessDocument(request);
    }
    
    private void PreProcessDocument(LifecycleRequest request)
    {
        // Add custom processing
    }
    
    private void PostProcessDocument(LifecycleRequest request)
    {
        // Add custom processing
    }
}
```

## Security Considerations

1. **HTTPS Only** - Always use HTTPS URLs for SigningWebApi
2. **Firewall** - Restrict network access to SigningWebApi
3. **Authentication** - Consider implementing OAuth or API key authentication
4. **Audit Logging** - Enable comprehensive logging for compliance
5. **Certificate Validation** - Verify certificate validity before signing
6. **User Permissions** - Control who can use signing activities
7. **Access Control** - Restrict access to sensitive documents

## Troubleshooting

### Activity Not Loading

**Issue:** Fusion P8 cannot find the activity

**Solutions:**
1. Verify DLL is in correct directory (`C:\Program Files\Fusion P8\CustomActivities\`)
2. Check assembly name in configuration matches file name
3. Verify .NET Framework 4.7.2 is installed
4. Restart Fusion P8 application server
5. Check Fusion P8 logs for detailed error messages

### Certificate Not Available

**Issue:** "No certificates with private keys found"

**Solutions:**
1. Verify certificates are installed in Windows Certificate Store
2. Run `Get-ChildItem Cert:\LocalMachine\My` in PowerShell
3. Ensure certificates have associated private keys
4. Check Fusion P8 service account has access to certificates

### Cannot Connect to SigningWebApi

**Issue:** "Connection refused" or timeout

**Solutions:**
1. Verify SigningWebApi is running on target server
2. Check firewall allows communication on API port
3. Verify URL is correct (includes protocol and port)
4. Test connectivity from Fusion P8 server to API server
5. Check SSL certificate validity (if using HTTPS)

### Signed Document Not Visible

**Issue:** Document appears unsigned in repository

**Solutions:**
1. Check activity logs for errors
2. Verify document was successfully saved to repository
3. Check file permissions on document storage
4. Verify document format supports signing

## Performance Optimization

- **Connection Pooling** - SigningApiClient reuses HTTP connections
- **Async Operations** - Use async/await for non-blocking operations
- **Document Streaming** - Large documents are streamed to avoid memory issues
- **Certificate Caching** - Cache certificate lists to reduce API calls

## Testing

### Manual Testing Steps

1. Create a document in Fusion P8
2. Submit to lifecycle with signing activity
3. Verify certificate selection dialog appears
4. Select a certificate
5. Verify signed document is returned
6. Check for visible stamp on document
7. Verify signature validity with external tool

### Automated Testing

Unit tests are available in `SigningWithCertsTests` project:

```bash
dotnet test SigningWithCertsTests
```

## Related Projects

- **Signing** - Core signing library and API client
- **SigningWebApi** - REST API for document signing
- **Stamping** - PDF stamping and visualization
- **Common** - Shared data models
- **SigningWithCertsTests** - Unit tests

## Version History

- **1.0.0** - Initial release with Fusion P8 lifecycle integration

## License

See repository LICENSE file.

## Support

For issues or questions:
1. Check the Troubleshooting section
2. Review Fusion P8 activity logs
3. Verify SigningWebApi connectivity
4. Contact the development team
5. Create an issue in the repository
