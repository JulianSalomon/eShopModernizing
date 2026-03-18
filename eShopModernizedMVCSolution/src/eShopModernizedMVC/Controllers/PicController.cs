using eShopModernizedMVC.Services;
using log4net;
using System;

using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Http;


namespace eShopModernizedMVC.Controllers
{
    public class PicController : Controller
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly string[] ValidFormats = { "image/jpeg", "image/png", "image/gif" };
        private readonly IImageService _imageService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PicController(ICatalogService service, IImageService imageService, IHttpContextAccessor httpContextAccessor)
        {
            _imageService = imageService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Route("uploadimage")]
        public ActionResult UploadImage()
        {
            _log.Info($"Now processing... /Pic/UploadImage");
            var image = _httpContextAccessor.HttpContext.Request.Form.Files["HelpSectionImages"];
            var itemId = _httpContextAccessor.HttpContext.Request.Form["itemId"];

            if (!IsValidImage(image))
            {
                return StatusCode((int)HttpStatusCode.BadRequest, "image is not valid");
            }

            int.TryParse(itemId, out var catalogItemId);
            var urlImageTemp = _imageService.UploadTempImage(image, catalogItemId);
            var tempImage = new
            {
                name = new Uri(urlImageTemp).PathAndQuery,
                url = urlImageTemp
            };

            return Json(tempImage);
        }

        private bool IsValidImage(IFormFile file)
        {
            bool isValidImage = true;
            try
            {
                isValidImage = file != null && ValidFormats.Contains(file.ContentType);
            }
            catch (Exception)
            {
                isValidImage = false;
            }

            return isValidImage;
        }

    }
}
