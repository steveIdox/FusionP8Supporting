using Files.idox.eim.fusionp8;
using idox.eim.fusionp8.supporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.AccessControl;
using Newtonsoft.Json;
using Common.Objects;

namespace SigningWithCertsTests
{
    internal class Program
    {
        private static string signingUrl = "https://localhost:7060/signing";

        static void Main(string[] args)
        {
            Console.WriteLine("Signing tests");
            var file = @"d:\\files\\pdf\\d.pdf";
            if(!System.IO.File.Exists(file))
            {
                Console.WriteLine($"File not found: {file}");
                return;
            }
            

            SigningApiClient signingApiClient = new SigningApiClient(signingUrl);

            var certs = signingApiClient.GetCertificatesWithPrivateKeysAsync().Result;

            Common.Objects.Certificate selectedCert = null;
            string thumbprint = null;

            foreach (Common.Objects.Certificate cert in certs)
            {
                int index = certs.IndexOf(cert);
                Console.Write($"Use this certificate? [{cert.subject}] (y/n): ");

                var key = Console.ReadKey(intercept: false);
                Console.WriteLine(); // ← THIS is what you're missing
                if (key.Key == ConsoleKey.Y)
                {
                    selectedCert = cert;
                    thumbprint = cert.thumbprint;
                    break;
                }
                Console.WriteLine();
            }
            if (selectedCert != null)
            {
                Common.Objects.StampConfiguration stampConfiguration = new Common.Objects.StampConfiguration();
                stampConfiguration.Pages = new int[] { 1 };
                stampConfiguration.VerticalAlignment = Common.Objects.VerticalStampAlignment.Bottom;
                stampConfiguration.HorizontalAlignment = Common.Objects.HorizontalStampAlignment.Center;

                Common.Objects.Document document = new Common.Objects.Document();
                document.Name = "d.pdf";
                document.Stream = new System.IO.FileStream(file, System.IO.FileMode.Open);
                document.Type = "application/pdf";

                //byte[] signedFile = Stamping.CertificateStamp.AddVisibleCertificateStampText(file, selectedCert, stampConfiguration);
                byte[] embddedSignatureFile = signingApiClient.SignPdfAsync(document.Stream,document.Name, thumbprint, stampConfiguration).Result;


                string tempFile = FileHelper.SaveToTempFile(embddedSignatureFile, file);
            }
        }
    }
}
