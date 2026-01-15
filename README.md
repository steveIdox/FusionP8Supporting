# FusionP8Supporting

Supporting libraries and utilities for Fusion P8 document management system integration.

## Overview

This repository contains a collection of .NET libraries that provide functionality for document signing, stamping, certificate management, and messaging within the Fusion P8 ecosystem.

## Projects

### Core Libraries

#### **Signing** (.NET Standard 2.0)
Certificate management and digital signature operations using Windows Certificate Store.

**Features:**
- Load certificates from Windows Certificate Store
- Sign content with RSA certificates
- Verify digital signatures
- Filter certificates by private key availability

**Key Classes:**
- `WindowsCertificateStore` - Certificate store operations

#### **Stamping** (.NET Standard 2.0)
PDF stamping and certificate embedding using Aspose.PDF.

**Features:**
- Apply text stamps to PDFs
- Apply image stamps to PDFs
- Add visible certificate information stamps
- Add invisible digital signatures to PDFs
- Configurable stamp positioning, rotation, opacity, font, and color

**Key Classes:**
- `Stamp` - Text and image stamping
- `CertificateStamp` - Certificate-based stamping
- `StampConfiguration` - Stamp appearance configuration

**Dependencies:**
- Aspose.PDF 25.3.0
- System.Configuration.ConfigurationManager

#### **Logging** (.NET Standard 2.0)
Centralized logging functionality.

**Key Classes:**
- `Log` - Logging operations

#### **Files** (.NET Standard 2.0)
File handling utilities.

**Features:**
- Save streams/bytes to temporary files
- Read streams to strings
- Generate temporary file paths
- Image file detection
- File deletion helpers

**Key Classes:**
- `FileHelper` - File operations

#### **Messaging** (.NET Standard 2.0)
RabbitMQ messaging integration.

**Features:**
- Publish messages to RabbitMQ exchanges
- Configurable connection settings

**Key Classes:**
- `RabbitMQPublisher` - Message publishing

#### **Common** (.NET Standard 2.0)
Shared data transfer objects and common types.

**Key Classes:**
- `Certificate` - Certificate DTO
- `Document` - Document DTO

#### **Format** (.NET Standard 2.0)
Data formatting utilities.

**Key Classes:**
- `Json` - JSON serialization helpers

### Web API

#### **SigningWebApi** (.NET 8.0)
REST API for document signing and certificate operations.

**Endpoints:**
- `GET /api/Signing/GetCertificates` - List all certificates
- `GET /api/Signing/GetCertificatesWithPrivateKeys` - List certificates with private keys
- `POST /api/Signing/Sign` - Sign a document with a certificate

**Key Classes:**
- `SigningController` - API endpoints
- `SigningHelper` - Signing operations
- `CertificateHelper` - Certificate operations

### Activities

#### **SampleActivities** (.NET Framework 4.7.2)
Fusion P8 lifecycle activity implementations.

**Activities:**
- `SignDocumentActivity` - Sign documents using Windows certificates
- `ActivityTemplate` - Base template for new activities

#### **SigningActivities** (.NET Framework 4.7.2)
Additional signing-related activities.

#### **CreateTransmittableContainer** (.NET Framework 4.7.2)
Container creation activities.

**Activities:**
- `CreateTransmittableContainerActivity` - Create logical document containers

### Test Projects

#### **SigningTests** (.NET Framework 4.7.2)
Unit tests for signing functionality.

#### **SigningWithCertsTests** (.NET Framework 4.7.2)
Integration tests for certificate-based signing.

#### **RabbitMQTests** (.NET Framework 4.7.2)
Tests for RabbitMQ messaging.

## Configuration

### Aspose.PDF License
Add to your app.config or web.config:
```xml
<appSettings>
  <add key="Aspose:License" value="path/to/Aspose.Total.lic" />
</appSettings>
