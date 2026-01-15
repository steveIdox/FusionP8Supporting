using System.Security.Cryptography.X509Certificates;
using System;

namespace Common.Objects
{
    public class Certificate
    {
        public string Thumbprint { get; set; }
        public string Subject { get; set; }
        public string SubjectAltName { get; set;}
        public string Issuer { get; set; }
        public DateTime NotBefore { get; set; }
        public DateTime NotAfter { get; set; }
        public string SerialNumber { get; set; }
        public bool HasPrivateKey { get; set; }
        public bool HasPublicKey { get; set; }
        public bool IsSelfSigned { get; set; }
        public bool IsExportable { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsRevocable { get; set; }
        public bool IsTrusted { get; set; }
        public bool IsArchived { get; set; }
        public bool IsMachineContext { get; set; }

        public Certificate(X509Certificate2 cert) {
            Thumbprint = cert.Thumbprint;
            Subject = cert.Subject;
            SubjectAltName = String.Empty;
            Issuer = cert.Issuer;
            NotBefore = cert.NotBefore;
            NotAfter = cert.NotAfter;
            SerialNumber = cert.SerialNumber;
            HasPrivateKey = cert.HasPrivateKey;
            HasPublicKey = false;
            IsSelfSigned = false;
            IsExportable = false;
            IsRevoked = false;
            IsRevocable = false;
            IsTrusted = false;
            IsArchived = false;
            IsMachineContext = false;
        }
    }
}
