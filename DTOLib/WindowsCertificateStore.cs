using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace idox.eim.fusionp8.DTOLib
{
    public class WindowsCertificateStore
    {
        public static X509Certificate2 LoadCert(string thumbprint) 
        { 
            Signing.
        }
        public static List<X509Certificate2> GetCertificates(StoreName storeName = StoreName.My, StoreLocation storeLocation = StoreLocation.LocalMachine) { }
        {
        }
        public static List<X509Certificate2> GetCertificatesWithPrivateKey(StoreName storeName = StoreName.My, StoreLocation storeLocation = StoreLocation.LocalMachine) { }
        {
        }
        public static byte[] SignContent(byte[] data, X509Certificate2 cert)
        {

        }
        public static bool VerifySignature(byte[] data,byte[] signature,X509Certificate2 cert)
        {

        }

    }
}
