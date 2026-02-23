# Stamping

A .NET Standard 2.0 library for applying digital stamps and timestamps to PDF documents.

## Overview

The Stamping library provides functionality to add visual stamps, timestamps, and certificate information to PDF documents. It supports configurable stamp placement, appearance, and content, making it suitable for document authentication and compliance workflows.

## Features

- **PDF Stamping** - Add visual stamps and text overlays to PDF documents
- **Certificate Stamps** - Include certificate information in document stamps
- **Configurable Appearance** - Customize stamp position, size, font, and colors
- **Digital Signatures** - Integrate certificate-based digital signatures with stamps
- **Cross-Platform** - Built on .NET Standard 2.0 for broad compatibility

## Prerequisites

- **.NET Standard 2.0** compatible platform
- **.NET Framework 4.7.2+**, **.NET Core 2.0+**, or **.NET 8+**

## Getting Started

### Build

```bash
dotnet build
```

### Basic Usage

```csharp
using Stamping;

// Create stamp configuration
var stampConfig = new StampConfiguration
{
    Text = "Approved",
    Position = "TopRight",
    FontSize = 12,
    Rotation = 45
};

// Apply stamp to PDF
var stamper = new Stamp();
stamper.ApplyStamp("input.pdf", "output.pdf", stampConfig);
```

## Project Structure

- **Stamp.cs** - Core stamping functionality for applying stamps to documents
- **CertificateStamp.cs** - Certificate-based stamping with signature information
- **CertificateSelector.cs** - Certificate selection and validation utilities
- **StampConfiguration.cs** - Configuration model for stamp appearance and behavior

## Dependencies

- **Aspose.PDF** (v25.3.0) - PDF manipulation and processing
- **FusionP8Supporting.Common** (v1.0.0) - Common utilities and helpers
- **System.Configuration.ConfigurationManager** (v8.0.1) - Configuration management
- **Signing** - Digital signature integration (project reference)

## Classes

### Stamp
Main class for applying visual stamps to PDF documents. Handles basic stamping operations.

### CertificateStamp
Extended stamping functionality with certificate information and digital signature integration.

### CertificateSelector
Utilities for selecting and validating certificates for use in stamping operations.

### StampConfiguration
Configuration model defining stamp appearance, position, text, fonts, and other properties.

## Configuration

Stamp behavior is controlled through the `StampConfiguration` class. Common configuration options include:

- **Text** - The text to display in the stamp
- **Position** - Location on the page (TopLeft, TopRight, BottomLeft, BottomRight, Center)
- **FontSize** - Size of stamp text in points
- **Rotation** - Rotation angle in degrees
- **Opacity** - Transparency level (0-100)
- **Color** - Text and border colors

## Integration with SigningWebApi

This library is used by `SigningWebApi` to provide document stamping endpoints. The API exposes REST endpoints that leverage the stamping functionality for automated document processing.

## Integration with Signing Library

Works seamlessly with the `Signing` library to apply both digital signatures and visible stamps to the same document.

## Development

### Adding New Stamp Types

To add new stamp types, extend the `Stamp` class or create new implementations:

```csharp
public class CustomStamp : Stamp
{
    public override void ApplyStamp(string inputFile, string outputFile, StampConfiguration config)
    {
        // Custom stamp implementation
    }
}
```

### Using with Certificates

For certificate-based stamping, use the `CertificateStamp` class:

```csharp
var certStamp = new CertificateStamp();
certStamp.ApplyStampWithCertificate(pdfPath, outputPath, certificate, config);
```

## Supported Platforms

- **.NET Framework 4.7.2+**
- **.NET Core 2.0+**
- **.NET 8+**
- **Mono**
- **Unity** (with limitations)

## License

See repository LICENSE file.

## Support

For issues or questions, refer to the project repository or documentation.
