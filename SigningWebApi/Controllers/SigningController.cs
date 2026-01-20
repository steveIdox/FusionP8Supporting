using idox.eim.fusionp8.supporting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;

namespace SigningWebApi.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class SigningController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public SigningController(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        [HttpGet("certificates")]
        public IEnumerable<Common.Objects.Certificate> GetCertificates()
        {
            return CertificateHelper.GetCertificates();
        }

        [HttpGet("certificates/private-keys")]
        public IEnumerable<Common.Objects.Certificate> GetCertificatesWithPrivateKeys()
        {
            return CertificateHelper.GetCertificatesWithPrivateKey();
        }
        [HttpPost("hash")]
        public async Task<IActionResult> Hash(IFormFile file, [FromForm] string thumbprint)
        {
            if (thumbprint == null) return BadRequest("Thumbprint not provided");
            if (file == null || file.Length == 0) return BadRequest("No file uploaded");

            var document = new Common.Objects.Document
            {
                Stream = file.OpenReadStream(),
                Name = file.FileName
            };

            var result = SigningHelper.Hash(document, thumbprint);

            // Return the stream as a file download instead of JSON
            return File(result.Stream, "application/octet-stream", result.Name);
        }
        [HttpPost("sign")]
        public async Task<IActionResult> sign(IFormFile file, string thumbprint)
        {
            if (thumbprint == null) return BadRequest("Thumbprint not provided");

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            Common.Objects.Document document = new Common.Objects.Document();
            document.Stream = file.OpenReadStream();
            document.Name = file.FileName;

            var result = SigningHelper.Hash(document, thumbprint);

            if (result == null) return BadRequest("Failed to sign document");

            return File(result.Stream, SigningHelper.GetContentType(file.Name), result.Name);
        }
        [HttpPost("signpdf")]
        public async Task<IActionResult> signpdf(IFormFile file, string thumbprint, 
            [FromForm] Common.Objects.StampConfiguration stampConfiguration,
            IFormFile ? stampImage = null)
        {
            if (thumbprint == null) return BadRequest("Thumbprint not provided");

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            Common.Objects.Document document = new Common.Objects.Document();
            document.Stream = file.OpenReadStream();
            document.Name = file.FileName;

            Common.Objects.Document stamp = null;
            if(stampImage != null)
            {
                stamp = new Common.Objects.Document();
                stamp.Stream = stampImage.OpenReadStream();
                stamp.Name = stampImage.FileName;
            }

            string licensePath = _configuration["Aspose:License"];
            var result = SigningHelper.SignPdf(document, thumbprint, licensePath, stamp, stampConfiguration);


            if (result == null) return BadRequest("Failed to sign PDF document");
            return File(result.Stream, SigningHelper.GetContentType(file.Name), result.Name);
        }
    }
}
