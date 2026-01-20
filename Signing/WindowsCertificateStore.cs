using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace idox.eim.fusionp8.supporting
{
    public class WindowsCertificateStore
    {
        public static X509Certificate2 LoadCert(string thumbprint)
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            try
            {
                var found = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
                if (found.Count == 0)
                    throw new InvalidOperationException($"Certificate with thumbprint {thumbprint} not found");
                return found[0];
            }
            finally
            {
                store.Close();
            }
        }

        public static List<X509Certificate2> GetCertificates(StoreName storeName = StoreName.My, StoreLocation storeLocation = StoreLocation.LocalMachine)
        {
            var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);

            try
            {
                var list = new List<X509Certificate2>();
                foreach (X509Certificate2 cert in store.Certificates)
                {
                    list.Add(cert);
                }
                return list;
            }
            finally
            {
                store.Close();
            }
        }

        public static List<X509Certificate2> GetCertificatesWithPrivateKey(StoreName storeName = StoreName.My, StoreLocation storeLocation = StoreLocation.LocalMachine)
        {
            var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);

            try
            {
                var list = new List<X509Certificate2>();
                foreach (X509Certificate2 cert in store.Certificates)
                {
                    if (cert.HasPrivateKey)
                        list.Add(cert);
                }
                return list;
            }
            finally
            {
                store.Close();
            }
        }

        //public static byte[] SignContent(byte[] data, X509Certificate2 cert)
        //{
        //    if (!cert.HasPrivateKey)
        //        throw new InvalidOperationException("Certificate does not have a private key");

        //    using (var rsa = cert.PrivateKey as RSACryptoServiceProvider)
        //    {
        //        if (rsa == null)
        //            throw new InvalidOperationException("Certificate does not have an RSA private key");

        //        return rsa.SignData(data, "SHA256");
        //    }
        //}

        public static bool VerifySignature(byte[] data, byte[] signature, X509Certificate2 cert)
        {
            using (var rsa = cert.PublicKey.Key as RSACryptoServiceProvider)
            {
                if (rsa == null)
                    throw new InvalidOperationException("Certificate does not have an RSA public key");

                return rsa.VerifyData(data, "SHA256", signature);
            }
        }
    }
}
