using idox.eim.fusionp8.supporting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;

namespace SigningWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SigningController : ControllerBase
    {
        public SigningController() { }

        [HttpGet(Name = "GetCertificates")]
        public IEnumerable<Common.Objects.Certificate> GetCertificates()
        {
            return CertificateHelper.GetCertificates();
        }


        [HttpGet(Name = "GetCertificatesWithPrivateKeys")]
        public IEnumerable<Common.Objects.Certificate> GetCertificatesWithPrivateKeys()
        {
            return CertificateHelper.GetCertificatesWithPrivateKey();
        }
        [HttpPost("Hash")]
        public async Task<IActionResult>Hash(IFormFile file, string thumbprint)
        {
            if(thumbprint == null) return BadRequest("Thumbprint not provided");

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            Common.Objects.Document document = new Common.Objects.Document();
            document.Stream = file.OpenReadStream();
            document.Name = file.FileName;

            SigningHelper.SignedHash(document, thumbprint);

            // Process the file
            return Ok();
        }
        [HttpPost("Sign")]
        public async Task<IActionResult> Sign(IFormFile file, string thumbprint)
        {
            if (thumbprint == null) return BadRequest("Thumbprint not provided");

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            Common.Objects.Document document = new Common.Objects.Document();
            document.Stream = file.OpenReadStream();
            document.Name = file.FileName;

            SigningHelper.SignDocument(document, thumbprint);

            // Process the file
            return Ok();
        }
    }
}
