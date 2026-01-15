using Files.idox.eim.fusionp8;
using idox.eim.fusionp8.supporting;
using System.Collections;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SigningWebApi
{
    public class SigningHelper
    {
        public SigningHelper()
        {
        }
        /// <summary>
        /// SignDocument
        /// Generate a HASH of the provided document
        /// </summary>
        /// <param name="SigningDTOs.Document"></param>
        /// <param name="thumbprint"></param>
        /// <returns>SigningDTOs.Document</returns>
        public static Common.Objects.Document SignedHash(Common.Objects.Document document, string thumbprint)
        {
            X509Certificate2 cert = WindowsCertificateStore.LoadCert(thumbprint);

            using var incrementalHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            byte[] buffer = new byte[8192];
            int bytesRead;

            while ((bytesRead = document.Stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                incrementalHash.AppendData(buffer, 0, bytesRead);
            }

            byte[] hash = incrementalHash.GetHashAndReset();
            byte[] signatureHash = WindowsCertificateStore.SignContent(hash, cert);

            return new Common.Objects.Document()
            {
                Stream = new MemoryStream(signatureHash),
                Name = document.Name
            };
        }
        public static Common.Objects.Document SignDocument(Common.Objects.Document document, string thumbprint)
        {
            Common.Objects.Certificate cert = new Common.Objects.Certificate(WindowsCertificateStore.LoadCert(thumbprint));
            Common.Objects.StampConfiguration stampConfiguration = new Common.Objects.StampConfiguration();

            byte[] stampedContent = Stamping.CertificateStamp.AddVisibleCertificateStampText(
                FileHelper.SaveToTempFile(document.Stream, FileHelper.GetTempFilePathFromInput(document.Name)),
                cert,
                stampConfiguration);

            return new Common.Objects.Document()
            {
                Stream = new MemoryStream(stampedContent),
                Name = document.Name
            };
        }
    }
}
