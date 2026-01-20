using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Common.Objects;

namespace idox.eim.fusionp8.supporting
{

    public class SigningApiClient : IDisposable
    {
        private readonly HttpClient _httpClient;

        public SigningApiClient(string baseUrl)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        }

        public async Task<List<Certificate>> GetCertificatesAsync()
        {
            var response = await _httpClient.GetAsync("api/Signing/certificates");
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Certificate>>(json);
        }

        public async Task<List<Certificate>> GetCertificatesWithPrivateKeysAsync()
        {
            var response = await _httpClient.GetAsync("api/Signing/certificates/private-keys");
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Certificate>>(json);
        }

        public async Task<byte[]> SignDocumentAsync(Stream fileStream, string fileName, string thumbprint)
        {
            var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            byte[] fileBytes = memoryStream.ToArray();

            var formData = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(fileBytes);

            fileContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/octet-stream");
            formData.Add(fileContent, "file", fileName);
            formData.Add(new StringContent(thumbprint), "thumbprint");

            var response = await _httpClient.PostAsync("api/Signing/sign", formData);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<byte[]> SignDocumentHashAsync(Stream fileStream, string fileName, string thumbprint, StampConfiguration stampConfiguration)
        {
            var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            byte[] fileBytes = memoryStream.ToArray();

            var formData = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(fileBytes);

            fileContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/octet-stream");
            formData.Add(fileContent, "file", fileName);
            formData.Add(new StringContent(thumbprint), "thumbprint");
            formData.Add(new StringContent(JsonConvert.SerializeObject(stampConfiguration)), "stampConfiguration");

            var response = await _httpClient.PostAsync("api/Signing/hash", formData);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<byte[]> SignPdfAsync(Stream fileStream, string fileName, string thumbprint, StampConfiguration stampConfiguration)
        {
            var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            byte[] fileBytes = memoryStream.ToArray();

            var formData = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(fileBytes);

            fileContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/pdf");
            formData.Add(fileContent, "file", fileName);
            formData.Add(new StringContent(thumbprint), "thumbprint");
            formData.Add(new StringContent(JsonConvert.SerializeObject(stampConfiguration)), "stampConfiguration");

            var response = await _httpClient.PostAsync("api/Signing/signpdf", formData);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }


        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}