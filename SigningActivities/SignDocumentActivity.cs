//  reference to signing project
using idox.eim.fusionp8.supporting;
//  required library from FusionP8
using Sword.BusinessApi.Lifecycles;
using Sword.Fusion.Api.Contents;
using Sword.Fusion.Api.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using Common.Objects;

namespace SampleActivities
{
    public class SignDocumentActivity : IActivity
    {
        private const string ActivityName = "SignDocumentActivity";

        public bool CanDo(ActivityExecutionContext context)
        {
            IPersistableObject activityObject = context.LifecycleObject;
            if (activityObject == null)
            {
                return false;
            }

            //  ensure we have a document
            var document = activityObject as IDocument;
            if (document == null)
            {
                return false;
            }

            //  check if document has content
            Sword.Fusion.Api.Contents.IFileContents contents =
                 document.RetrieveContents<Sword.Fusion.Api.Contents.IFileContents>();
            if (contents == null)
            {
                Logging.Log.Error("Empty contents.",ActivityName);
                return false;
            }
            
            return true;
        }

        public void Do(ActivityExecutionContext context)
        {
            if (context == null) { Logging.Log.Error("NULL context", ActivityName); return; }
            if (context.LifecycleObject == null) { Logging.Log.Error("NULL lifecycle object", ActivityName); return; }

            //  get our selected doc
            var currentDocument = context.LifecycleObject as IDocument;
            if (currentDocument == null)
            {
                Logging.Log.Error($"Object is not a document",ActivityName);
                return;
            }

            try
            {
                var baseUrl = "https://localhost:44300/";

                //  crfeate instance of SigningWebApiClient
                var client = new SigningApiClient(baseUrl);

                //  get list of certificates with private keys (needed for signing)
                List<Common.Objects.Certificate> certificates = client.GetCertificatesWithPrivateKeysAsync().Result;

                if (certificates == null || certificates.Count == 0)
                {
                    Logging.Log.Error($"No certificates with private keys found in the certificate store",ActivityName);
                    return;
                }

                //  display available certificates to user
                Logging.Log.Info($"{ActivityName}: Available certificates:", ActivityName);
                for (int i = 0; i < certificates.Count; i++)
                {
                    var cert = certificates[i];
                    Logging.Log.Info($"  [{i + 1}] Subject: {cert.subject}",ActivityName);
                    Logging.Log.Info($"      Issuer: {cert.issuer}",ActivityName);
                    Logging.Log.Info($"      Thumbprint: {cert.thumbprint}",ActivityName);
                    Logging.Log.Info($"      Valid From: {cert.notBefore} To: {cert.notAfter}",ActivityName);
                }

                //  prompt user to select a certificate
                Logging.Log.Info($"Select a certificate (1-{certificates.Count}): ", ActivityName);
                string input = Console.ReadLine();

                if (!int.TryParse(input, out int selection) || selection < 1 || selection > certificates.Count)
                {
                    Logging.Log.Error($"Invalid selection", ActivityName);
                    return;
                }

                var selectedCert = certificates[selection - 1];
                Logging.Log.Info($"Selected certificate: {selectedCert.subject}", ActivityName);

                //  get document content
                Sword.Fusion.Api.Contents.IFileContents contents =
                    currentDocument.RetrieveContents<Sword.Fusion.Api.Contents.IFileContents>();

                //  create a HASH from the content
                byte[] signature = client.SignDocumentAsync(contents.FileElement.GetStream(),
                    contents.FileElement.FileName,
                    selectedCert.thumbprint).Result;

                //  store the signature (you can customize how to store it based on your requirements)
                //  Option A: Store as a properties
                currentDocument.Properties["DigitalSignature"] =  Convert.ToBase64String(signature);
                currentDocument.Properties["SignatureCertificateThumbprint"] =  selectedCert.thumbprint;
                currentDocument.Properties["SignatureDate"] =  DateTime.UtcNow.ToString("o");

                //  Option B: Store as a file
                //  var signatureFile = new Sword.Fusion.Api.Contents.FileContents
                //  {
                //      FileElement = new Sword.Fusion.Api.Contents.FileElement

                Logging.Log.Info($"Document signed successfully", ActivityName);
                Logging.Log.Info($"Signature stored as base64 string in 'DigitalSignature' property", ActivityName);

            }
            catch (Exception ex)
            {
                Logging.Log.Error($"Error signing document - {ex.Message}", ActivityName);
                Logging.Log.Error($"{ex.StackTrace}", ActivityName);
            }
        }
    }
}