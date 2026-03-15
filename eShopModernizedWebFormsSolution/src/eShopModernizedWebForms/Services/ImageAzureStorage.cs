using eShopModernizedWebForms.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;


namespace eShopModernizedWebForms.Services
{
    public class ImageAzureStorage : IImageService
    {

        private readonly BlobServiceClient _blobServiceClient;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageAzureStorage(IWebHostEnvironment webHostEnvironment)
        {
            _blobServiceClient = new BlobServiceClient("YourAzureStorageConnectionString");
            _webHostEnvironment = webHostEnvironment;
        }

        public string BaseUrl()
        {
            return _blobServiceClient.Uri.ToString();
        }

        public string BuildUrlImage(CatalogItem item)
        {
            if (string.IsNullOrEmpty(item.PictureFileName))
                return UrlDefaultImage();

            return _blobServiceClient.Uri + "pics/" + item.Id + "/" + item.PictureFileName;
        }

        public void Dispose()
        {
        }

        public void InitializeCatalogImages()
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient("pics");

            containerClient.CreateIfNotExists(PublicAccessType.Blob);

            var existingBlobs = containerClient.GetBlobs();
            Parallel.ForEach(existingBlobs, blobItem =>
            {
                containerClient.GetBlobClient(blobItem.Name).Delete();
            });

            var webRoot = Path.Combine(_webHostEnvironment.WebRootPath, "Pics");

            for (int i = 1; i <= 12; i++)
            {
                var path = Path.Combine(webRoot, i + ".png");
                var blobName = i + "/" + i + ".png";
                UpLoadImageFromFile(containerClient, blobName, path, "image/png");

            }
            var defaultImagePath = Path.Combine(webRoot, "default.png");
            UpLoadImageFromFile(containerClient, "temp/default.png", defaultImagePath, "image/png");
        }

        public void UpdateImage(CatalogItem item)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient("pics");

            var folder = item.TempImageName.Replace("/pics/", string.Empty);

            BlobClient tempBlob = containerClient.GetBlobClient(folder);

            var existingBlobs = containerClient.GetBlobs(prefix: item.Id + "/");
            foreach (var blobItem in existingBlobs)
            {
                containerClient.GetBlobClient(blobItem.Name).Delete();
            }

            var fileName = Path.GetFileName(item.TempImageName);
            BlobClient imageBlob = containerClient.GetBlobClient(item.Id + "/" + fileName);

            imageBlob.StartCopyFromUri(tempBlob.Uri);
            tempBlob.Delete();
        }

        public string UploadTempImage(IFormFile file, int? catalogItemId)
        {
            string path = catalogItemId.HasValue ? catalogItemId + "/temp/" : "temp/" + Guid.NewGuid().ToString() + "/";

            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient("pics");
            BlobClient blobClient = containerClient.GetBlobClient(path + file.FileName.ToLower());

            var uploadOptions = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType }
            };
            var stream = file.OpenReadStream();
            stream.Seek(0, SeekOrigin.Begin);
            blobClient.Upload(stream, uploadOptions);

            return blobClient.Uri.ToString();
        }

        public string UrlDefaultImage()
        {
            return _blobServiceClient.Uri + "pics/temp/default.png";
        }

        private void UpLoadImageFromFile(BlobContainerClient containerClient, string blobName, string filePath, string contentType)
        {
            var fileStream = File.OpenRead(filePath);
            fileStream.Seek(0, SeekOrigin.Begin);

            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            var uploadOptions = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
            };
            blobClient.Upload(fileStream, uploadOptions);
        }


    }
}
