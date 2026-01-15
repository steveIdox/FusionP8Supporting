using Files.idox.eim.fusionp8;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using idox.eim.fusionp8.supporting;


namespace SigningTests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Signing tests");
            var file = @"d:\\files\\pdf\\d.pdf";

            List<X509Certificate2> certs = WindowsCertificateStore.GetCertificatesWithPrivateKey();
            X509Certificate2 selectedCert = null;
            string thumbprint = null;

            foreach (X509Certificate2 cert in certs)
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

            //  removed - SelectCertificate is not avialable in netstandard2.0
            //X509Certificate2 selectedCert2 = idox.eim.fusionp8.supporting.WindowsCertificateStore.SelectCertificate();

            if (selectedCert != null)
            {
                Console.WriteLine("Selected certificate: " + selectedCert.Subject);
                Console.WriteLine($"Checking  thumbprint [" + thumbprint + "] can be used to obtain the same certificate");
                X509Certificate2 cert2 = WindowsCertificateStore.LoadCert(thumbprint);
                if(cert2.Equals(cert2)) { Console.WriteLine("Success"); }
                else { Console.WriteLine("Failed"); return; }

                    byte[] sig = WindowsCertificateStore.SignContent(FileHelper.bytesFromFile(file), selectedCert);
                string stringSig = Convert.ToBase64String(sig);
                Console.WriteLine("Signature: " + stringSig);

                Console.WriteLine("Checks");

                Console.WriteLine("Verify Orginal file");
                bool c1 = WindowsCertificateStore.VerifySignature(FileHelper.bytesFromFile(file), sig, selectedCert);
                Console.WriteLine($"Verifying signature with [{file}] and [{stringSig}] = [{c1}]");

                //  mess with sig (bytes)                
                Console.WriteLine("Verify bad signature for original file");
                byte[] sig2 = new byte[sig.Length];
                sig.CopyTo(sig2, 0);
                sig2[1] = 0;
                string stringSig2 = Convert.ToBase64String(sig2);
                bool c2 = WindowsCertificateStore.VerifySignature(FileHelper.bytesFromFile(file), sig2, selectedCert);
                Console.WriteLine($"Verifying signature with [{file}] and [{stringSig2}] = [{c2}]");

                // mess with file (bytes)
                Console.WriteLine("Verify alternate file with good signature");
                string file2 = @"d:\\files\\pdf\\e.pdf";
                bool c3 = WindowsCertificateStore.VerifySignature(FileHelper.bytesFromFile(file), sig2, selectedCert);
                Console.WriteLine($"Verifying signature with [{file2}] and [{stringSig}] = [{c3}]");
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
