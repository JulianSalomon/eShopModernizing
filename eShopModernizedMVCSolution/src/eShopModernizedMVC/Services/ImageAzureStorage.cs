using eShopModernizedMVC.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;


namespace eShopModernizedMVC.Services
{
public class ImageAzureStorage
    {

        private readonly string _accountName;
        private readonly string _accountKey;
        private readonly Uri _blobStoragePrimaryUri;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private static readonly HttpClient _httpClient = new HttpClient();

        public ImageAzureStorage(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _accountName = "youraccount";
            _accountKey = "yourkey";
            _blobStoragePrimaryUri = new Uri($"https://{_accountName}.blob.core.windows.net/");
            _httpContextAccessor = httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
        }

        public string BaseUrl()
        {
            return _blobStoragePrimaryUri.ToString();
        }

        public string BuildUrlImage(CatalogItem item)
        {
            if (string.IsNullOrEmpty(item.PictureFileName))
                return UrlDefaultImage();

            return _blobStoragePrimaryUri + "pics/" + item.Id + "/" + item.PictureFileName;
        }

        public void Dispose()
        {
        }

        public void InitializeCatalogImages()
        {
            CreateContainerIfNotExists("pics").GetAwaiter().GetResult();
            SetContainerPublicAccess("pics").GetAwaiter().GetResult();

            var blobs = ListBlobs("pics", string.Empty).GetAwaiter().GetResult();
            foreach (var blobName in blobs)
            {
                DeleteBlob("pics", blobName).GetAwaiter().GetResult();
            }

            var webRoot = Path.Combine(_webHostEnvironment.WebRootPath, "Pics");

            for (int i = 1; i <= 12; i++)
            {
                var path = Path.Combine(webRoot, i + ".png");
                var blobName = i + "/" + i + ".png";
                UpLoadImageFromFile("pics", blobName, path, "image/png");
            }
            var defaultImagePath = Path.Combine(webRoot, "default.png");
            UpLoadImageFromFile("pics", "temp/default.png", defaultImagePath, "image/png");
        }

        public void UpdateImage(CatalogItem item)
        {
            var folder = item.TempImageName.Replace("/pics/", string.Empty);
            var tempBlobUri = new Uri(_blobStoragePrimaryUri, "pics/" + folder);

            var existingBlobs = ListBlobs("pics", item.Id + "/").GetAwaiter().GetResult();
            foreach (var blobName in existingBlobs)
            {
                DeleteBlob("pics", blobName).GetAwaiter().GetResult();
            }

            var fileName = Path.GetFileName(item.TempImageName);
            var destBlobName = item.Id + "/" + fileName;
            CopyBlob("pics", folder, destBlobName).GetAwaiter().GetResult();
            DeleteBlob("pics", folder).GetAwaiter().GetResult();
        }

        public string UploadTempImage(IFormFile file, int? catalogItemId)
        {
            string path = catalogItemId.HasValue ? catalogItemId + "/temp/" : "temp/" + Guid.NewGuid().ToString() + "/";
            var blobName = path + file.FileName.ToLower();

using var stream = file.OpenReadStream();
            stream.Seek(0, SeekOrigin.Begin);
            UploadBlob("pics", blobName, stream, file.ContentType).GetAwaiter().GetResult();

            return _blobStoragePrimaryUri + "pics/" + blobName;
        }

        public string UrlDefaultImage()
        {
            return _blobStoragePrimaryUri + "pics/temp/default.png";
        }

        private void UpLoadImageFromFile(string containerName, string blobName, string filePath, string contentType)
        {
using var fileStream = File.OpenRead(filePath);
            fileStream.Seek(0, SeekOrigin.Begin);
            UploadBlob(containerName, blobName, fileStream, contentType).GetAwaiter().GetResult();
        }

        private async Task CreateContainerIfNotExists(string containerName)
        {
            var uri = new Uri(_blobStoragePrimaryUri, $"{containerName}?restype=container");
            var request = new HttpRequestMessage(HttpMethod.Put, uri);
            request.Content = new ByteArrayContent(Array.Empty<byte>());
            request.Content.Headers.ContentLength = 0;
            AddAuthorizationHeader(request, "PUT", containerName, string.Empty, string.Empty, 0, string.Empty);
            var response = await _httpClient.SendAsync(request);
            // 201 = created, 409 = already exists - both are acceptable
        }

        private async Task SetContainerPublicAccess(string containerName)
        {
            var uri = new Uri(_blobStoragePrimaryUri, $"{containerName}?restype=container&comp=acl");
            var request = new HttpRequestMessage(HttpMethod.Put, uri);
            request.Headers.Add("x-ms-blob-public-access", "blob");
            request.Content = new ByteArrayContent(Array.Empty<byte>());
            request.Content.Headers.ContentLength = 0;
            AddAuthorizationHeader(request, "PUT", containerName, string.Empty, string.Empty, 0, string.Empty);
            await _httpClient.SendAsync(request);
        }

        private async Task<List<string>> ListBlobs(string containerName, string prefix)
        {
            var uriStr = $"{_blobStoragePrimaryUri}{containerName}?restype=container&comp=list";
            if (!string.IsNullOrEmpty(prefix))
                uriStr += $"&prefix={Uri.EscapeDataString(prefix)}";
            var uri = new Uri(uriStr);
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            AddAuthorizationHeader(request, "GET", containerName, string.Empty, string.Empty, -1, string.Empty);
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var blobNames = new List<string>();
            if (response.IsSuccessStatusCode)
            {
                var xml = XDocument.Parse(content);
                blobNames = xml.Descendants("Name").Select(x => x.Value).ToList();
            }
            return blobNames;
        }

        private async Task DeleteBlob(string containerName, string blobName)
        {
            var uri = new Uri(_blobStoragePrimaryUri, $"{containerName}/{blobName}");
            var request = new HttpRequestMessage(HttpMethod.Delete, uri);
            AddAuthorizationHeader(request, "DELETE", containerName, blobName, string.Empty, -1, string.Empty);
            await _httpClient.SendAsync(request);
        }

        private async Task UploadBlob(string containerName, string blobName, Stream data, string contentType)
        {
            var uri = new Uri(_blobStoragePrimaryUri, $"{containerName}/{blobName}");
            var request = new HttpRequestMessage(HttpMethod.Put, uri);
            request.Headers.Add("x-ms-blob-type", "BlockBlob");
            var bytes = ReadAllBytes(data);
            request.Content = new ByteArrayContent(bytes);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            AddAuthorizationHeader(request, "PUT", containerName, blobName, contentType, bytes.Length, string.Empty);
            await _httpClient.SendAsync(request);
        }

        private async Task CopyBlob(string containerName, string sourceBlobName, string destBlobName)
        {
            var uri = new Uri(_blobStoragePrimaryUri, $"{containerName}/{destBlobName}");
            var request = new HttpRequestMessage(HttpMethod.Put, uri);
            var copySource = $"{_blobStoragePrimaryUri}{containerName}/{sourceBlobName}";
            request.Headers.Add("x-ms-copy-source", copySource);
            request.Content = new ByteArrayContent(Array.Empty<byte>());
            request.Content.Headers.ContentLength = 0;
            AddAuthorizationHeader(request, "PUT", containerName, destBlobName, string.Empty, 0, string.Empty);
            await _httpClient.SendAsync(request);
        }

        private byte[] ReadAllBytes(Stream stream)
        {
using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }

        private void AddAuthorizationHeader(HttpRequestMessage request, string verb, string containerName, string blobName, string contentType, long contentLength, string contentMD5)
        {
            var date = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
            request.Headers.Add("x-ms-date", date);
            request.Headers.Add("x-ms-version", "2020-04-08");

            var resource = blobName.Length > 0
                ? $"/{_accountName}/{containerName}/{blobName}"
                : $"/{_accountName}/{containerName}";

            if (request.RequestUri.Query.Contains("restype=container") || request.RequestUri.Query.Contains("comp="))
            {
                resource = $"/{_accountName}/{containerName}\n" + BuildCanonicalizedQuery(request.RequestUri.Query);
            }

            var canonicalizedHeaders = BuildCanonicalizedHeaders(request);

            var stringToSign = BuildStringToSign(verb, contentLength, contentMD5, contentType, date, canonicalizedHeaders, resource);

            var signature = SignString(stringToSign);
            request.Headers.Authorization = new AuthenticationHeaderValue("SharedKey", $"{_accountName}:{signature}");
        }

        private string BuildCanonicalizedHeaders(HttpRequestMessage request)
        {
            var headers = request.Headers
                .Where(h => h.Key.StartsWith("x-ms-", StringComparison.OrdinalIgnoreCase))
                .OrderBy(h => h.Key.ToLowerInvariant())
                .Select(h => $"{h.Key.ToLowerInvariant()}:{string.Join(",", h.Value)}");
            return string.Join("\n", headers);
        }

        private string BuildCanonicalizedQuery(string query)
        {
            if (string.IsNullOrEmpty(query)) return string.Empty;
            query = query.TrimStart('?');
            var parts = query.Split('&')
                .Select(p => p.Split('='))
                .Where(p => p.Length == 2)
                .OrderBy(p => p[0])
                .Select(p => $"{p[0]}:{Uri.UnescapeDataString(p[1])}");
            return string.Join("\n", parts);
        }

        private string BuildStringToSign(string verb, long contentLength, string contentMD5, string contentType, string date, string canonicalizedHeaders, string canonicalizedResource)
        {
            var contentLengthStr = contentLength >= 0 ? contentLength.ToString() : string.Empty;
            return string.Join("\n", verb, contentMD5, contentType, string.Empty,
                canonicalizedHeaders, canonicalizedResource);
        }

        private string SignString(string stringToSign)
        {
            var keyBytes = Convert.FromBase64String(_accountKey);
using var hmac = new HMACSHA256(keyBytes);
            var signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
            return Convert.ToBase64String(signatureBytes);
        }
    }
}
