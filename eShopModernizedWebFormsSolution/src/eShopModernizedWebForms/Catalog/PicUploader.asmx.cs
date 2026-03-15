using Autofac;

using eShopModernizedWebForms.Services;
using log4net;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;


using System.Text.Json;
using Microsoft.AspNetCore.Http;


namespace eShopModernizedWebForms.Catalog
{
    /// <summary>
    /// Summary description for PicUploader
    /// </summary>
    //[WebService(Namespace = "http://tempuri.org/")]
    //[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    //[System.ComponentModel.ToolboxItem(false)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.

    public class PicUploader
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static ImageFormat[] ValidFormats = new[] { ImageFormat.Jpeg, ImageFormat.Png, ImageFormat.Gif };

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IImageService _imageService;

        public PicUploader(IHttpContextAccessor httpContextAccessor, IImageService imageService)
        {
            _httpContextAccessor = httpContextAccessor;
            _imageService = imageService;
        }

        public void UploadImage()
        {
            _log.Info($"Now Processing... /Catalog/PicUploader.asmx");

            var httpContext = _httpContextAccessor.HttpContext;

            IFormFile image = httpContext.Request.Form.Files["HelpSectionImages"];
            var itemId = httpContext.Request.Form["itemId"];

            if (!IsValidImage(image))
            {
                httpContext.Response.StatusCode = 400;
                return;
            }

            int.TryParse(itemId, out var catalogItemId);
            var urlImageTemp = _imageService.UploadTempImage(image, catalogItemId);
            var tempImage = new
            {
                name = new Uri(urlImageTemp).PathAndQuery,
                url = urlImageTemp
            };
            var jsonResult = JsonSerializer.Serialize(tempImage);
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.WriteAsync(jsonResult);
        }

        private bool IsValidImage(IFormFile file)
        {
            bool isValidImage = true;
            try
            {
using (var img = Image.FromStream(file.OpenReadStream()))
                {
                    isValidImage = false;
                    foreach (var format in ValidFormats)
                    {
                        if (img.RawFormat == format)
                        {
                            isValidImage = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                isValidImage = false;
            }

            return isValidImage;
        }
    }
}
