using Files.idox.eim.fusionp8;
using idox.eim.fusionp8.supporting;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SigningWebApi
{
    public class SigningHelper
    {
        public SigningHelper()
        {
        }
        public static Common.Objects.Document SignFile(Common.Objects.Document document, string thumbprint)
        {
            X509Certificate2 cert = WindowsCertificateStore.LoadCert(thumbprint);

            // Read file content
            document.Stream.Position = 0;
            byte[] fileContent = new byte[document.Stream.Length];
            document.Stream.Read(fileContent, 0, fileContent.Length);

            // Sign the content using RSA
            using (RSA rsa = cert.GetRSAPrivateKey())
            {
                byte[] signature = rsa.SignData(fileContent, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                return new Common.Objects.Document()
                {
                    Stream = new MemoryStream(signature),
                    Name = document.Name + ".sig"
                };
            }
        }

        public static Common.Objects.Document Hash(Common.Objects.Document document, string thumbprint)
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

        public static byte[] GetHash(Common.Objects.Document document)
        {
            using var incrementalHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            byte[] buffer = new byte[8192];
            int bytesRead;

            while ((bytesRead = document.Stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                incrementalHash.AppendData(buffer, 0, bytesRead);
            }

            return incrementalHash.GetHashAndReset();
        }

        private static string TempCertificateImage(X509Certificate2 cert)
        {
            string tempImagePath = Path.GetTempFileName() + ".png";

            var bmp = new Bitmap(600, 200, PixelFormat.Format32bppArgb);

            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                var titleFont = new Font("Arial", 16, FontStyle.Bold);
                var bodyFont = new Font("Arial", 14);
                var smallFont = new Font("Arial", 12);

                g.DrawString("Digitally signed by:", titleFont, Brushes.Black, 10, 10);
                g.DrawString(cert.Subject, bodyFont, Brushes.Black, 10, 50);
                g.DrawString(DateTime.Now.ToString(), smallFont, Brushes.Black, 10, 100);


                bmp.Save(tempImagePath, ImageFormat.Png);

                return tempImagePath;
            }
        }
        private static Rectangle CalculateSignatureRectangle(
            Aspose.Pdf.Page page,
            int width,
            int height,
            Common.Objects.StampConfiguration cfg,
            int margin = 20)
        {
            float x;
            float y;

            // Horizontal
            switch (cfg.HorizontalAlignment)
            {
                case Common.Objects.HorizontalStampAlignment.Left:
                    x = margin;
                    break;
                case Common.Objects.HorizontalStampAlignment.Center:
                    x = (float)(page.Rect.Width / 2 - width / 2);
                    break;
                case Common.Objects.HorizontalStampAlignment.Right:
                    x = (float)(page.Rect.Width - width - margin);
                    break;
                default:
                    x = margin;
                    break;
            }

            // Vertical (PDF origin is bottom-left!)
            switch (cfg.VerticalAlignment)
            {
                case Common.Objects.VerticalStampAlignment.Top:
                    y = (float)(page.Rect.Height - height - margin);
                    break;
                case Common.Objects.VerticalStampAlignment.Middle:
                    y = (float)(page.Rect.Height / 2 - height / 2);
                    break;
                case Common.Objects.VerticalStampAlignment.Bottom:
                    y = margin;
                    break;
                default:
                    y = margin;
                    break;
            }

            return new Rectangle((int)x, (int)y, width, height);
        }

        public static string GetContentType(string fileName)
        {
            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            return provider.TryGetContentType(fileName, out string contentType) ? contentType : "application/octet-stream";
        }

        public static Common.Objects.Document SignPdf(Common.Objects.Document document, 
            string thumbprint, 
            string licensePath, 
            Common.Objects.Document stamp,
            Common.Objects.StampConfiguration cfg = null)
        {
            if (!document.Name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                return null;

            X509Certificate2 cert = WindowsCertificateStore.LoadCert(thumbprint);

            // Save stream to temp file (Aspose needs file path)
            string tempInputFile = FileHelper.SaveToTempFile(document.Stream, document.Name);
            string tempStampFile = FileHelper.SaveToTempFile(stamp.Stream, stamp.Name);
            if(tempStampFile == null) tempStampFile= TempCertificateImage(cert);

            Common.Objects.StampConfiguration stampConfiguration; stampConfiguration = null; if (cfg == null) cfg = new Common.Objects.StampConfiguration();

            //  now we want to embed the cert into the PDF
                // Set Aspose license
            Aspose.Pdf.License license = new Aspose.Pdf.License();
            try
            {
                license.SetLicense(licensePath);
            }
            catch (Exception)
            {
                return null;
            }

            Aspose.Pdf.Page firstPage = null;
            try
            {
                Aspose.Pdf.Document tempPdf = new Aspose.Pdf.Document(tempInputFile);
                if(tempPdf.Pages.Count > 0) firstPage = tempPdf.Pages[1];
                else return null;
            }
            catch(Exception)
            { 
                return null; 
            }

            try
            {

                // Export cert to temporary PFX
                string tempPfxFile = System.IO.Path.GetTempFileName() + ".pfx";
                string pfxPassword = "temp123";
                byte[] pfxBytes = cert.Export(X509ContentType.Pfx, pfxPassword);
                System.IO.File.WriteAllBytes(tempPfxFile, pfxBytes);

                try
                {
                    // Sign the PDF
                    var pdfSign = new Aspose.Pdf.Facades.PdfFileSignature();
                    pdfSign.BindPdf(tempInputFile);

                    // Create signature rectangle (visible signature)
                    var rect = CalculateSignatureRectangle(firstPage, 600, 200, stampConfiguration);

                    // Replace the problematic signature creation with:
                    var signature = new Aspose.Pdf.Forms.PKCS7(tempPfxFile, pfxPassword);
                    pdfSign.SignatureAppearance = tempStampFile;

                    // Then use this overload:
                    pdfSign.Sign(1, "reason", "contact", "Office Location", true, rect, signature);

                    string tempOutputFile = FileHelper.GetTempFilePathWithExtension(".pdf");
                    pdfSign.Save(tempOutputFile);

                    byte[] signedPdfBytes = System.IO.File.ReadAllBytes(tempOutputFile);

                    return new Common.Objects.Document()
                    {
                        Stream = new MemoryStream(signedPdfBytes),
                        Name = document.Name
                    };
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
                finally
                {
                    FileHelper.DeleteFile(tempPfxFile);
                }
            }
            finally
            {
                FileHelper.DeleteFile(tempInputFile);
            }
        }
    }
}
