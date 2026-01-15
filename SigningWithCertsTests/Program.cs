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

namespace SigningWithCertsTests
{
    internal class Program
    {
        private static string signingUrl = "https://localhost:5001/signing";

        static void Main(string[] args)
        {
            Console.WriteLine("Signing tests");
            var file = @"d:\\files\\pdf\\d.pdf";

            var httpClient = new HttpClient { BaseAddress = new Uri(signingUrl) }; // Update port

            var response = httpClient.GetAsync("api/Signing/GetCertificatesWithPrivateKeys").Result;
            var json = response.Content.ReadAsStringAsync().Result;
            var certs = JsonConvert.DeserializeObject<List<Common.Objects.Certificate>>(json);

            Common.Objects.Certificate selectedCert = null;
            string thumbprint = null;

            foreach (Common.Objects.Certificate cert in certs)
            {
                int index = certs.IndexOf(cert);
                Console.Write($"Use this certificate? [{cert.Subject}] (y/n): ");

                var key = Console.ReadKey(intercept: false);
                Console.WriteLine(); // ← THIS is what you're missing
                if (key.Key == ConsoleKey.Y)
                {
                    selectedCert = cert;
                    thumbprint = cert.Thumbprint;
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

                string tempFile = FileHelper.SaveToTempFile(
                    Stamping.CertificateStamp.AddVisibleCertificateStampText(file, selectedCert, stampConfiguration),
                    file);
            }
        }
    }
}
