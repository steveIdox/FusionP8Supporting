using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Files.idox.eim.fusionp8;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using idox.eim.fusionp8.supporting;
// uses aspose
using Aspose.Pdf;
using Aspose.Pdf.Forms;
using Aspose.Pdf.Facades;

namespace Stamping
{
    public class CertificateStamp
    {
        private static string TemporaryPfxPassword = "fusionp8certpass";
        public static byte[] SimpleTextStampFromCertificate(string file, string thumbprint, Common.Objects.StampConfiguration stampConfiguration)
        {
            var cert = WindowsCertificateStore.LoadCert(thumbprint);
            string certInfo = $"Signed by: {cert.Subject}\nValid until: {cert.NotAfter:yyyy-MM-dd}";
            return Stamp.ApplyText(file, certInfo, stampConfiguration);
        }
        
        public static byte[] SimpleTextStampFromCertificate(string file, X509Certificate2 cert, Common.Objects.StampConfiguration stampConfiguration)
        {
            string certInfo = $"Signed by: {cert.Subject}\nValid until: {cert.NotAfter:yyyy-MM-dd}";
            return Stamp.ApplyText(file, certInfo, stampConfiguration);
        }

        public static byte[] AddVisibleCertificateStampText(string file, Common.Objects.Certificate cert, Common.Objects.StampConfiguration stampConfiguration)
        {
            // Use the working text stamp approach with enhanced certificate info
            //X509Certificate2 x509Certificate2 = WindowsCertificateStore.LoadCert(cert.Thumbprint);
            string certInfo = $"DIGITALLY SIGNED\nSigned by: {cert.subject}\nIssuer: {cert.issuer}\nValid until: {cert.notAfter:yyyy-MM-dd}\nThumbprint: {cert.thumbprint}";
            return Stamp.ApplyText(file, certInfo, stampConfiguration);
        }
        public static byte[] AddVisibleCertificateStampImage(string file, string stampFile, Common.Objects.Certificate cert, Common.Objects.StampConfiguration stampConfiguration)
        {
            // Use the working text stamp approach with enhanced certificate info
            string certInfo = $"DIGITALLY SIGNED\nSigned by: {cert.subject}\nIssuer: {cert.issuer}\nValid until: {cert.notAfter:yyyy-MM-dd}\nThumbprint: {cert.thumbprint}";
            return Stamp.ApplyImage(file, stampFile, stampConfiguration);
        }
        public static byte[] AddVisibleCertificateStampImage(string file, Common.Objects.Certificate cert, Common.Objects.StampConfiguration stampConfiguration)
        {
            // Use the working text stamp approach with enhanced certificate info
            string certInfo = $"DIGITALLY SIGNED\nSigned by: {cert.subject}\nIssuer: {cert.issuer}\nValid until: {cert.notAfter:yyyy-MM-dd}\nThumbprint: {cert.thumbprint}";
            string imageFile = CreateImageFromText(certInfo, stampConfiguration);
            return Stamp.ApplyImage(file, imageFile, stampConfiguration);
        }
        private static string CreateImageFromText(string text, Common.Objects.StampConfiguration config)
        {
            string[] lines = text.Split('\n');

            using (var bitmap = new System.Drawing.Bitmap(600, lines.Length * 30 + 20))
            using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
            {
                graphics.Clear(System.Drawing.Color.White);

                var font = new System.Drawing.Font(config.FontName ?? "Arial", config.FontSize);
                var brush = new System.Drawing.SolidBrush(System.Drawing.ColorTranslator.FromHtml("#" + config.FontColor));

                float y = 10;
                foreach (var line in lines)
                {
                    graphics.DrawString(line, font, brush, 10, y);
                    y += 30;
                }

                string tempImageFile = FileHelper.GetTempFilePathWithExtension(".png");
                bitmap.Save(tempImageFile, System.Drawing.Imaging.ImageFormat.Png);

                return tempImageFile;
            }
        }


        public static byte[] AddInvisibleCertificateStamp(string file, X509Certificate2 cert, Common.Objects.StampConfiguration stampConfiguration)
        {            Aspose.Pdf.License license = new Aspose.Pdf.License();
            license.SetLicense(System.Configuration.ConfigurationManager.AppSettings["Aspose:License"]);

            if (!cert.HasPrivateKey)
                throw new InvalidOperationException("Certificate must have a private key for signing");

            string tempPfxFile = String.Empty;
            try 
            { 
                var pdf = new Aspose.Pdf.Document(file);

                // Export certificate to temporary PFX file
                tempPfxFile = System.IO.Path.GetTempFileName() + ".pfx";
                byte[] pfxBytes = cert.Export(X509ContentType.Pfx, TemporaryPfxPassword);
                System.IO.File.WriteAllBytes(tempPfxFile, pfxBytes);

                var pkcs7 = new Aspose.Pdf.Forms.PKCS7(tempPfxFile, TemporaryPfxPassword);

                var pdfSignature = new PdfFileSignature();
                pdfSignature.BindPdf(file);

                // 🔑 THIS is the correct call for 25.3.0
                pdfSignature.Sign(1, cert.Subject, cert.SerialNumber,cert.Issuer,false,new System.Drawing.Rectangle(0, 0, 0, 0));

                string tempFile = FileHelper.GetTempFilePathFromInput(file);
                pdf.Save(tempFile);
                return FileHelper.bytesFromFile(tempFile);
            }
            finally
            {
                // Clean up temporary PFX file
                if (System.IO.File.Exists(tempPfxFile))
                    System.IO.File.Delete(tempPfxFile);
            }
        }
    }
}
