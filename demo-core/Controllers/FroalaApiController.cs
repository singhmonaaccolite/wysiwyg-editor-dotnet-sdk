using System;
using Microsoft.AspNetCore.Mvc;
using ImageMagick;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace demo.Controllers
{
    public class FroalaApiController : Controller
    {
        public IActionResult UploadImage()
        {
            string uploadPath = "wwwroot/uploads/";

            try
            {  
                return Json(FroalaEditor.Image.Upload(HttpContext, uploadPath));
            }
            catch (Exception e)
            {
                return Json(e);
            }
        }

        public IActionResult UploadVideo()
        {
            string uploadPath = "wwwroot/uploads/";

            try
            {
                return Json(FroalaEditor.Video.Upload(HttpContext, uploadPath));
            }
            catch (Exception e)
            {
                return Json(e);
            }
        }

        public IActionResult UploadFile () {
            string uploadPath = "wwwroot/uploads/";

            object response;
            try
            {  
                response = FroalaEditor.File.Upload(HttpContext, uploadPath);
                return Json(response);
            }
            catch (Exception e)
            {
               return Json(e);
            }
        }

        public IActionResult LoadImages()
        {
            string uploadPath = "wwwroot/uploads/";

            try
            {  
                return Json(FroalaEditor.Image.List(uploadPath));
            }
            catch (Exception e)
            {
                return Json(e);
            }
        }

        public IActionResult UploadFilesManager () {
            string uploadPath = "wwwroot/uploads/";

            object response;
            try
            {  
                response = FroalaEditor.FilesManager.Upload(HttpContext, uploadPath);
                return Json(response);
            }
            catch (Exception e)
            {
               return Json(e);
            }
        }

        public IActionResult UploadImageResize(Microsoft.AspNetCore.Http.IFormFile file)
        {
            string fileRoute = "wwwroot/uploads/";
            var fileMimeType = file.ContentType;
            string[] ImageMimeTypes = new string[] { "image/gif", "image/jpeg", "audio/mpeg", "image/png", "image/webp" };
            if (!ImageMimeTypes.Contains(fileMimeType.ToString()))
            {
                return Json(new Exception("Invalid contentType. It must be Image"));
            }

            MagickGeometry resizeGeometry = new MagickGeometry(300, 300);
            resizeGeometry.IgnoreAspectRatio = true;

            FroalaEditor.ImageOptions options = new FroalaEditor.ImageOptions
            {
                ResizeGeometry = resizeGeometry
            };

            try
            {
                return Json(FroalaEditor.Image.Upload(HttpContext, fileRoute, options));
            }
            catch (Exception e)
            {
                return Json(e);
            }
        }

        public IActionResult UploadImageValidation ()
        {
            string fileRoute = "wwwroot/uploads/";

            Func<string, string, bool> validationFunction = (filePath, mimeType) => {

                MagickImageInfo info = new MagickImageInfo(filePath);

                if (info.Width != info.Height)
                {
                    return false;
                }

                return true;
            };

            FroalaEditor.ImageOptions options = new FroalaEditor.ImageOptions
            {
                Fieldname = "myImage",
                Validation = new FroalaEditor.ImageValidation(validationFunction)
            };

            try
            {
                return Json(FroalaEditor.Image.Upload(HttpContext, fileRoute, options));
            }
            catch (Exception e)
            {
                return Json(e);
            }
        }

        public IActionResult UploadFileValidation ()
        {
            string fileRoute = "wwwroot/";

            Func<string, string, bool> validationFunction = (filePath, mimeType) => {

                long size = new System.IO.FileInfo(filePath).Length;
                if (size > 10 * 1024 * 1024)
                {
                    return false;
                }

                return true;
            };

            FroalaEditor.FileOptions options = new FroalaEditor.FileOptions
            {
                Fieldname = "myFile",
                Validation = new FroalaEditor.FileValidation(validationFunction)
            };

            try
            {
                return Json(FroalaEditor.Image.Upload(HttpContext, fileRoute, options));
            }
            catch (Exception e)
            {
                return Json(e);
            }
        }

        public IActionResult UploadFilesManagerValidation (Microsoft.AspNetCore.Http.IFormFile file)
        {
            string fileRoute = "wwwroot/";
            var fileMimeType = file.ContentType;
            string[] ImageMimeTypes = new string[] { "image/gif", "image/jpeg", "audio/mpeg", "image/png", "image/webp" };
            if(!ImageMimeTypes.Contains(fileMimeType.ToString()))
            {
                return Json(new Exception("Invalid contentType. It must be Image"));
            }

            Func<string, string, bool> validationFunction = (filePath, mimeType) => {

                long size = new System.IO.FileInfo(filePath).Length;
                if (size > 10 * 1024 * 1024)
                {
                    return false;
                }
                MagickImageInfo info = new MagickImageInfo(filePath);

                if (info.Width != info.Height)
                {
                    return false;
                }

                return true;
            };

            FroalaEditor.FileOptions options = new FroalaEditor.FileOptions
            {
                Fieldname = "file",
                Validation = new FroalaEditor.FileValidation(validationFunction)
            };

            try
            {
                return Json(FroalaEditor.Image.Upload(HttpContext, fileRoute, options));
            }
            catch (Exception e)
            {
                return Json(e);
            }
        }

        public IActionResult DeleteFile ()
        {
            try
            {
                FroalaEditor.File.Delete(HttpContext.Request.Form["src"]);
                return Json(true);
            }
            catch (Exception e)
            {
                return Json(e);
            }
        }

        public IActionResult DeleteVideo()
        {
            try
            {
                FroalaEditor.Video.Delete(HttpContext.Request.Form["src"]);
                return Json(true);
            }
            catch (Exception e)
            {
                return Json(e);
            }
        }


        public IActionResult DeleteImage()
        {
            try
            {
                FroalaEditor.Image.Delete(HttpContext.Request.Form["src"]);
                return Json(true);
            }
            catch (Exception e)
            {
                return Json(e);
            }
        }

        public IActionResult S3Signature ()
        {
            FroalaEditor.S3Config config = new FroalaEditor.S3Config
            {
                Bucket = Environment.GetEnvironmentVariable("AWS_BUCKET"),
                Region = Environment.GetEnvironmentVariable("AWS_REGION"),
                KeyStart = Environment.GetEnvironmentVariable("AWS_KEY_START"),
                Acl = Environment.GetEnvironmentVariable("AWS_ACL"),
                AccessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY"),
                SecretKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY"),
                Expiration = Environment.GetEnvironmentVariable("AWS_EXPIRATION") // Expiration s3 image signature #11
            };

            return Json(FroalaEditor.S3.GetHash(config));
        }

        public IActionResult Azure ()
        {
            return Json( new Dictionary<string, object>
            {
                { "account", Environment.GetEnvironmentVariable("AZURE_ACCOUNT") },
                { "accessKey", Environment.GetEnvironmentVariable("AZURE_ACCESS_KEY") },
                { "container", Environment.GetEnvironmentVariable("AZURE_CONTAINER") },
                { "SASToken", Environment.GetEnvironmentVariable("AZURE_SAS_TOKEN") },
                { "uploadURL", Environment.GetEnvironmentVariable("AZURE_UPLOAD_URL") },
            });
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
