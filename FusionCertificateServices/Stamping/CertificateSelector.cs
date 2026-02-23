using System;
using System.Security.Cryptography.X509Certificates;

namespace Stamping
{
    public class CertificateSelector
    {
        //public static X509Certificate2 SelectCertificate()
        //{
        //    var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        //    store.Open(OpenFlags.ReadOnly);

        //    try
        //    {
        //        var collection = X509Certificate2UI.SelectFromCollection(
        //            store.Certificates,
        //            "Select Certificate",
        //            "Choose a certificate for signing:",
        //            X509SelectionFlag.SingleSelection);

        //        return collection.Count > 0 ? collection[0] : null;
        //    }
        //    finally
        //    {
        //        store.Close();
        //    }
        //}
    }
}