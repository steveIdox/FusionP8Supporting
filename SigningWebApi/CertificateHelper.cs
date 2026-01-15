using System.Security.Cryptography.X509Certificates;

namespace SigningWebApi
{
    public class CertificateHelper
    {
        public static List<Common.Objects.Certificate> GetCertificates()
        {
         List<Common.Objects.Certificate> certificates = new List<Common.Objects.Certificate>();
            foreach(X509Certificate2 x509 in 
                idox.eim.fusionp8.supporting.WindowsCertificateStore.GetCertificates())
                certificates.Add(new Common.Objects.Certificate(x509)); // create a new certificate

            return certificates;
        }
        public static List<Common.Objects.Certificate> GetCertificatesWithPrivateKey()
        {
            List<Common.Objects.Certificate> certificates = new List<Common.Objects.Certificate>();
            foreach (X509Certificate2 x509 in
                idox.eim.fusionp8.supporting.WindowsCertificateStore.GetCertificatesWithPrivateKey())
                certificates.Add(new Common.Objects.Certificate(x509)); // create a new certificate

            return certificates;
        }
    }
}
