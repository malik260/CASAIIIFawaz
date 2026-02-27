using Core.DTOs;
using Logic.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CASA3.Controllers
{
    [Authorize]
    public class NewsLetterController : Controller
    {
        private readonly INewsLetterService _newsLetterService;
        private readonly ICloudinaryService _cloudinaryService;

        public NewsLetterController(INewsLetterService newsLetterService, ICloudinaryService cloudinaryService)
        {
            _newsLetterService = newsLetterService;
            _cloudinaryService = cloudinaryService;
        }

        public IActionResult Index()
        {
            var newsletters = _newsLetterService.GetAllNewsLetterService();
            return View(newsletters);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewsLetter([FromForm] NewsletterDto model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Title) || string.IsNullOrEmpty(model.Author))
                    return Json(new { success = false, message = "Title and Author are required." });

                if (model.CoverImage == null || model.CoverImage.Length == 0)
                    return Json(new { success = false, message = "Cover Image is required." });

                if (model.Document == null || model.Document.Length == 0)
                    return Json(new { success = false, message = "Document is required." });

                // Validate and upload cover image
                if (model.CoverImage.Length > 5 * 1024 * 1024)
                    return Json(new { success = false, message = "Cover image size exceeds 5MB limit." });

                var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var imageExt = Path.GetExtension(model.CoverImage.FileName).ToLowerInvariant();
                if (!Array.Exists(allowedImageExtensions, e => e == imageExt))
                    return Json(new { success = false, message = "Invalid image type. Only JPG, JPEG, PNG, GIF, and WEBP files are allowed." });

                var coverImageUrl = await _cloudinaryService.UploadImageAsync(model.CoverImage, "casa3/newsletter-images");
                if (string.IsNullOrEmpty(coverImageUrl))
                    return Json(new { success = false, message = "Failed to upload cover image." });

                // Validate and upload document
                if (model.Document.Length > 10 * 1024 * 1024)
                    return Json(new { success = false, message = "Document file size exceeds 10MB limit." });

                var allowedDocExtensions = new[] { ".pdf", ".doc", ".docx" };
                var docExt = Path.GetExtension(model.Document.FileName).ToLowerInvariant();
                if (!Array.Exists(allowedDocExtensions, e => e == docExt))
                    return Json(new { success = false, message = "Invalid document type. Only PDF, DOC, and DOCX files are allowed." });

                var documentUrl = await _cloudinaryService.UploadRawAsync(model.Document, "casa3/newsletter-documents");
                if (string.IsNullOrEmpty(documentUrl))
                    return Json(new { success = false, message = "Failed to upload document." });

                var result = await _newsLetterService.CreateNewsLetterservice(model, coverImageUrl, documentUrl);
                return Json(result);
            }
            catch
            {
                return Json(new { success = false, message = "An error occurred while creating the newsletter. Please try again." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetNewsLetterById(string id)
        {
            var result = await _newsLetterService.GetNewsLetterByIdService(id);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateNewsLetter([FromForm] NewsletterUpdateDto model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Id) || string.IsNullOrEmpty(model.Title) || string.IsNullOrEmpty(model.Author))
                    return Json(new { success = false, message = "ID, Title and Author are required." });

                string? coverImageUrl = null;
                string? documentUrl = null;

                if (model.CoverImage != null && model.CoverImage.Length > 0)
                {
                    if (model.CoverImage.Length > 5 * 1024 * 1024)
                        return Json(new { success = false, message = "Cover image size exceeds 5MB limit." });

                    var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var imageExt = Path.GetExtension(model.CoverImage.FileName).ToLowerInvariant();
                    if (!Array.Exists(allowedImageExtensions, e => e == imageExt))
                        return Json(new { success = false, message = "Invalid image type. Only JPG, JPEG, PNG, GIF, and WEBP files are allowed." });

                    coverImageUrl = await _cloudinaryService.UploadImageAsync(model.CoverImage, "casa3/newsletter-images");
                    if (string.IsNullOrEmpty(coverImageUrl))
                        return Json(new { success = false, message = "Failed to upload cover image." });
                }

                if (model.Document != null && model.Document.Length > 0)
                {
                    if (model.Document.Length > 10 * 1024 * 1024)
                        return Json(new { success = false, message = "Document file size exceeds 10MB limit." });

                    var allowedDocExtensions = new[] { ".pdf", ".doc", ".docx" };
                    var docExt = Path.GetExtension(model.Document.FileName).ToLowerInvariant();
                    if (!Array.Exists(allowedDocExtensions, e => e == docExt))
                        return Json(new { success = false, message = "Invalid document type. Only PDF, DOC, and DOCX files are allowed." });

                    documentUrl = await _cloudinaryService.UploadRawAsync(model.Document, "casa3/newsletter-documents");
                    if (string.IsNullOrEmpty(documentUrl))
                        return Json(new { success = false, message = "Failed to upload document." });
                }

                var result = await _newsLetterService.UpdateNewsLetterService(model, coverImageUrl, documentUrl);
                return Json(result);
            }
            catch
            {
                return Json(new { success = false, message = "An error occurred while updating the newsletter. Please try again." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNewsLetter(string id)
        {
            var result = await _newsLetterService.DeleteNewsLetterByIdService(id);
            return Json(result);
        }
    }
}
