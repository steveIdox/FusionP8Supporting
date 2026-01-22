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
using System.IO;

namespace SigningWithCertsTests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Signing tests");
            var testFile = System.Configuration.ConfigurationManager.AppSettings["TestFile"];
            if(!System.IO.File.Exists(testFile))
            {
                Console.WriteLine($"File not found: [{testFile}]");
                return;
            }
            Console.WriteLine($"Using TestFile [{testFile}]");

            var signingUrl = System.Configuration.ConfigurationManager.AppSettings["SigningWebApiUrl"];
            if(string.IsNullOrEmpty(signingUrl))
            {
                Console.WriteLine($"Signing WebApi URL not valid: [{signingUrl}]");
                return;
            }
            Console.WriteLine($"Signing WebApi URL [{signingUrl}]");

            SigningApiClient signingApiClient = new SigningApiClient(signingUrl);

            var certs = signingApiClient.GetCertificatesWithPrivateKeysAsync().Result;
            if (certs == null || certs.Count == 0)
            {
                Console.WriteLine($"No suitable Certificates not found");
                return;
            }

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

                FileInfo testFileInfo = new FileInfo(testFile);
                var tempFileName = $"temp-{testFileInfo.Name}";

                var tempFilePath = Path.Combine(testFileInfo.Directory.FullName, tempFileName);

                Common.Objects.Document document = new Common.Objects.Document();
                document.Name = testFileInfo.Name;
                document.Stream = new System.IO.FileStream(testFile, System.IO.FileMode.Open);
                document.Type = "application/pdf";

                //byte[] signedFile = Stamping.CertificateStamp.AddVisibleCertificateStampText(file, selectedCert, stampConfiguration);
                byte[] embddedSignatureFile = signingApiClient.SignPdfAsync(document.Stream,document.Name, thumbprint, stampConfiguration).Result;


                var tempFile = FileHelper.SaveToTempFile(embddedSignatureFile, tempFileName);
            }
        }
    }
}
