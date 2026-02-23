using System.Security.Cryptography.X509Certificates;
using System;
using Newtonsoft.Json;

namespace Common.Objects
{
    public class Certificate
    {
        [JsonProperty("thumbprint")]
        public string thumbprint { get; set; }

        [JsonProperty("subject")]
        public string subject { get; set; }

        [JsonProperty("subjectAltName")]
        public string subjectAltName { get; set; }

        [JsonProperty("issuer")]
        public string issuer { get; set; }

        [JsonProperty("notBefore")]
        public DateTimeOffset notBefore { get; set; }

        [JsonProperty("notAfter")]
        public DateTimeOffset notAfter { get; set; }

        [JsonProperty("serialNumber")]
        public string serialNumber { get; set; }

        [JsonProperty("hasPrivateKey")]
        public bool hasPrivateKey { get; set; }

        [JsonProperty("hasPublicKey")]
        public bool hasPublicKey { get; set; }

        [JsonProperty("isSelfSigned")]
        public bool isSelfSigned { get; set; }

        [JsonProperty("isExportable")]
        public bool isExportable { get; set; }

        [JsonProperty("isRevoked")]
        public bool isRevoked { get; set; }

        [JsonProperty("isRevocable")]
        public bool isRevocable { get; set; }

        [JsonProperty("isTrusted")]
        public bool isTrusted { get; set; }

        [JsonProperty("isArchived")]
        public bool isArchived { get; set; }

        [JsonProperty("isMachineContext")]
        public bool isMachineContext { get; set; }
        public Certificate() { }

        public Certificate(X509Certificate2 cert) {
            thumbprint = cert.Thumbprint;
            subject = cert.Subject;
            subjectAltName = String.Empty;
            issuer = cert.Issuer;
            notBefore = cert.NotBefore;
            notAfter = cert.NotAfter;
            serialNumber = cert.SerialNumber;
            hasPrivateKey = cert.HasPrivateKey;
            hasPublicKey = false;
            isSelfSigned = false;
            isExportable = false;
            isRevoked = false;
            isRevocable = false;
            isTrusted = false;
            isArchived = false;
            isMachineContext = false;
        }
    }
}
