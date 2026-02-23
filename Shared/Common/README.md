# Common Library

Core foundational library providing common data models, utilities, and cross-cutting concerns used across all FusionP8Supporting projects.

## Overview

The Common library (.NET Standard 2.0) provides:
- **Objects** - Shared data models (Certificate, Document, StampConfiguration)
- **Logging** - Unified logging abstraction
- **Files** - File and stream utilities
- **Format** - Data formatting helpers (WebApi, Json)

## Target Framework

The Common library targets **.NET Standard 2.0**, making it compatible with:
- .NET Framework 4.7.2+
- .NET Core/.NET 5+
- .NET 8.0

## Library Reference

### Objects

Provides shared data models used throughout the solution.

#### Certificate Class

Represents an X.509 digital certificate with comprehensive metadata.

```csharp
using Common.Objects;

public class Certificate
{
    public string thumbprint { get; set; }           // Unique certificate identifier
    public string subject { get; set; }              // Certificate subject (CN=...)
    public string subjectAltName { get; set; }       // Alternative names
    public string issuer { get; set; }               // Issuing authority
    public DateTimeOffset notBefore { get; set; }    // Valid from date
    public DateTimeOffset notAfter { get; set; }     // Expiration date
    public string serialNumber { get; set; }         // Serial number
    public bool hasPrivateKey { get; set; }          // Has private key for signing
    public bool hasPublicKey { get; set; }           // Has public key
    public bool isSelfSigned { get; set; }           // Self-signed certificate
    public bool isExportable { get; set; }           // Can be exported
    public bool isRevoked { get; set; }              // Revocation status
    public bool isRevocable { get; set; }            // Can be revoked
    public bool isTrusted { get; set; }              // Trusted root
    public bool isArchived { get; set; }             // Archived certificate
    public bool isMachineContext { get; set; }       // Machine vs user context
}

// Create from X509Certificate2
var cert = new Certificate(x509Certificate2);
```

#### Document Class

Represents a document with stream and metadata.

```csharp
using Common.Objects;

public class Document
{
    public Stream Stream { get; set; }    // Document content as stream
    public string Name { get; set; }      // File name
    public string Type { get; set; }      // MIME type or extension
}

// Usage
var doc = new Document
{
    Stream = fileStream,
    Name = "contract.pdf",
    Type = "application/pdf"
};
```

#### StampConfiguration Class

Controls visual stamp appearance on PDF documents.

```csharp
using Common.Objects;

public class StampConfiguration
{
    public HorizontalStampAlignment HorizontalAlignment { get; set; }  // Left/Center/Right
    public VerticalStampAlignment VerticalAlignment { get; set; }      // Top/Middle/Bottom
    public double Opacity { get; set; }                                // 0.0 to 1.0
    public double Rotation { get; set; }                               // Degrees
    public int[] Pages { get; set; }                                   // Which pages (null = all)
    public int FontSize { get; set; }                                  // Default: 36
    public string FontName { get; set; }                               // Default: "Arial"
    public string FontColor { get; set; }                              // Hex color, default: "#FF0000"
}

public enum HorizontalStampAlignment { Left, Center, Right }
public enum VerticalStampAlignment { Top, Middle, Bottom }

// Usage
var config = new StampConfiguration
{
    HorizontalAlignment = HorizontalStampAlignment.Center,
    VerticalAlignment = VerticalStampAlignment.Middle,
    Opacity = 0.7,
    FontSize = 48,
    FontColor = "#0000FF"
};
```

### Logging

Provides unified logging abstraction for the application.

**Dependencies:**
- Microsoft.CSharp (v4.7.0)
- System.Data.DataSetExtensions (v4.5.0)

#### Log Class

Static logging facade with three levels.

```csharp
using Logging;

public class Log
{
    public static void Info(string message, string activityName);
    public static void Error(string message, string activityName);
    public static void Debug(string message, string activityName);
}

// Usage
Log.Info("Processing document", "SigningActivity");
Log.Error("Certificate not found", "SigningActivity");
Log.Debug("Debug information", "SigningActivity");

// Output format: "ActivityName: message"
```

### Files

File and stream utilities for document processing.

**Dependencies:**
- Microsoft.CSharp (v4.7.0)
- System.Data.DataSetExtensions (v4.5.0)

#### FileHelper Class

```csharp
using Files.idox.eim.fusionp8;

public class FileHelper
{
    // Delete a file
    public static void DeleteFile(string filePath);
    
    // Save stream to temp file
    public static string SaveToTempFile(Stream inputStream, string fileName);
    
    // Save bytes to temp file
    public static string SaveToTempFile(byte[] bytes, string fileName);
    
    // Save bytes to specific location
    public static bool SaveTo(byte[] bytes, string fileName);
    
    // Read entire stream to string
    public static string ReadStream(Stream stream);
    
    // Add suffix to filename before extension
    public static string AddToFileName(string inputPath, string addToName);
    
    // Get temp file path with extension
    public static string GetTempFilePathWithExtension(string extension);
    
    // Get temp file path from input file
    public static string GetTempFilePathFromInput(string filePath);
}

// Usage
// Save file to temp location
string tempPath = FileHelper.SaveToTempFile(fileStream, "document.pdf");

// Read stream content
string content = FileHelper.ReadStream(myStream);

// Create unique temp path
string uniquePath = FileHelper.GetTempFilePathWithExtension(".pdf");

// Cleanup
FileHelper.DeleteFile(tempPath);
```

### Format

Data formatting helpers:
- **WebApi** - Web API formatting utilities
- **Json** - JSON formatting using Newtonsoft.Json

**Dependencies:**
- Newtonsoft.Json (v13.0.4)

#### Json Class

JSON formatting helpers.

```csharp
using Format;

public class Json
{
    public static string Format(string value);
    public static string Format(string[] values);
}

// Usage
string json = Format.Json.Format("myValue");
string jsonArray = Format.Json.Format(new[] { "val1", "val2" });
```

#### WebApi Class

Web API formatting utilities.

```csharp
using Format;

public class WebApi
{
    // Format repository and document IDs
    public static string Format(string repositoryId, string documentId);
}

// Usage
string identifier = Format.WebApi.Format("FP8Repo", "DOC123");
// Result: "FP8Repo_DOC123"
```

## Dependency Graph

```
Signing (NetStandard 2.0)
├── Common

SigningWebApi (.NET 8.0)
├── Common
├── Files
├── Logging
├── Signing
└── Stamping

SigningActivities (.NET Framework 4.7.2)
├── Common
├── Files
├── Logging
└── Signing

Stamping (NetStandard 2.0)
├── Common
├── Files
├── Logging
└── Signing

Messaging (NetStandard 2.0)
└── Logging

PushToRabbitMQActivity (.NET Framework 4.7.2)
└── Messaging

Format (NetStandard 2.0)
```

## Usage Patterns

### Pattern 1: Data Model Transfer

Use `Common.Objects` classes for transferring data between layers.

```csharp
// API Layer
var doc = new Document
{
    Stream = request.Files[0].InputStream,
    Name = request.Files[0].FileName,
    Type = "application/pdf"
};

// Business Layer
var result = SigningHelper.Hash(doc, thumbprint);

// Return to client
return File(result.Stream, "application/octet-stream", result.Name);
```

### Pattern 2: Logging Integration

Add context logging throughout your code.

```csharp
public void ProcessDocument(Document document)
{
    Log.Info($"Processing: {document.Name}", "DocumentProcessor");
    
    try
    {
        // Process document
        Log.Debug("Document processed successfully", "DocumentProcessor");
    }
    catch (Exception ex)
    {
        Log.Error(ex.Message, "DocumentProcessor");
        throw;
    }
}
```

### Pattern 3: File Operations

Use `FileHelper` for consistent file handling.

```csharp
public byte[] SignPdf(Stream fileStream, string fileName, string thumbprint)
{
    // Save to temp location
    string tempFile = FileHelper.SaveToTempFile(fileStream, fileName);
    
    try
    {
        // Process file
        var cert = WindowsCertificateStore.LoadCert(thumbprint);
        byte[] signature = SignDocument(tempFile, cert);
        
        return signature;
    }
    finally
    {
        // Cleanup
        FileHelper.DeleteFile(tempFile);
    }
}
```

### Pattern 4: PDF Stamping

Apply visual marks to PDFs during signing.

```csharp
public byte[] SignAndStampPdf(Document document, Certificate cert)
{
    var stampConfig = new StampConfiguration
    {
        HorizontalAlignment = HorizontalStampAlignment.Center,
        VerticalAlignment = VerticalStampAlignment.Bottom,
        Opacity = 0.8,
        FontSize = 24
    };
    
    // Save to temp and stamp
    string tempFile = FileHelper.SaveToTempFile(document.Stream, document.Name);
    byte[] stamped = CertificateStamp.AddVisibleCertificateStampText(
        tempFile,
        cert,
        stampConfig);
    
    FileHelper.DeleteFile(tempFile);
    return stamped;
}
```

## NuGet Package References

All shared libraries reference these NuGet packages:

| Package | Version | Used By |
|---------|---------|---------|
| Newtonsoft.Json | 13.0.4 | Common, Format |
| Microsoft.CSharp | 4.7.0 | Logging, Files |
| System.Data.DataSetExtensions | 4.5.0 | Logging, Files |
| Aspose.PDF | 25.3.0 | Stamping |
| System.Configuration.ConfigurationManager | 8.0.1 | Stamping |

## Testing

Shared libraries are tested indirectly through:
- **SigningWithCertsTests** - Certificate and signing functionality
- **RabbitMQTests** - Messaging functionality

## Best Practices

### 1. Use Common Objects for Data Transfer

```csharp
// ✓ Good
public byte[] SignDocument(Document document, Certificate cert)
{
    // Implementation
}

// ✗ Avoid
public byte[] SignDocument(FileStream stream, X509Certificate2 cert)
{
    // Implementation
}
```

### 2. Consistent Logging

```csharp
// ✓ Good - Clear activity context
Log.Error("Certificate not found", "SigningActivity");

// ✗ Avoid - Missing context
Log.Error("Certificate not found");
```

### 3. Proper File Cleanup

```csharp
// ✓ Good - Ensures cleanup
string tempFile = FileHelper.SaveToTempFile(stream, name);
try
{
    // Process
}
finally
{
    FileHelper.DeleteFile(tempFile);
}

// ✗ Avoid - May leak temp files
string tempFile = FileHelper.SaveToTempFile(stream, name);
// Process without guaranteed cleanup
```

### 4. Configuration Over Hardcoding

```csharp
// ✓ Good - Configurable
var config = new StampConfiguration
{
    FontSize = int.Parse(settings["StampFontSize"]),
    FontColor = settings["StampFontColor"]
};

// ✗ Avoid - Hardcoded values
var config = new StampConfiguration
{
    FontSize = 36,
    FontColor = "#FF0000"
};
```

## Migration Guide

When updating a project to use these shared libraries:

1. **Add Project References**
   ```xml
   <ProjectReference Include="..\Common\Common.csproj" />
   <ProjectReference Include="..\Logging\Logging.csproj" />
   ```

2. **Update Using Statements**
   ```csharp
   using Common.Objects;
   using Logging;
   using Files.idox.eim.fusionp8;
   ```

3. **Replace Local Models**
   Replace project-specific models with shared `Common.Objects` classes

4. **Implement Logging**
   Add `Log.Info/Error/Debug` calls throughout code

## Related Solutions

- **Signing Solution** - Leverages Common, Files, Logging, Stamping
- **Messaging Solution** - Leverages Logging, Format
- **Sample Activities** - Demonstrates shared library usage

## License

See repository LICENSE file.

## Support

For shared library issues:
1. Check this documentation
2. Review example code in dependent projects
3. Contact the development team
4. Create an issue in the repository
