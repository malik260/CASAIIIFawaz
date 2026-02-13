using Core.DTOs;
using Core.Enum;
using Logic.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CASA3.Controllers
{
    [Authorize]
    public class CarouselController : Controller
    {
        private readonly ICarouselService _carouselService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CarouselController(ICarouselService carouselService, IWebHostEnvironment webHostEnvironment)
        {
            _carouselService = carouselService;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var carousels = _carouselService.GetAllCarouselsService();
            return View(carousels);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCarousel([FromForm] CarouselDto model)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(model.Title))
                {
                    return Json(new { success = false, message = "Title is required." });
                }

                if (string.IsNullOrEmpty(model.ButtonText))
                {
                    return Json(new { success = false, message = "Button Text is required." });
                }

                // Validate Background Image
                if (model.BackgroundImage == null || model.BackgroundImage.Length == 0)
                {
                    return Json(new { success = false, message = "Background Image is required." });
                }

                // Validate based on page type
                if (model.PageType == CarouselPageType.Home)
                {
                    if (model.Brochure == null || model.Brochure.Length == 0)
                    {
                        return Json(new { success = false, message = "Brochure (PDF) is required for Home carousel." });
                    }
                }

                string backgroundImageUrl = null;
                string brochureUrl = null;

                // Handle Background Image upload
                if (model.BackgroundImage != null && model.BackgroundImage.Length > 0)
                {
                    // Validate file size (5MB for background images)
                    if (model.BackgroundImage.Length > 5 * 1024 * 1024)
                    {
                        return Json(new { success = false, message = "Background image size exceeds 5MB limit." });
                    }

                    // Validate image file extension
                    var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var imageExtension = Path.GetExtension(model.BackgroundImage.FileName).ToLowerInvariant();
                    if (!Array.Exists(allowedImageExtensions, ext => ext == imageExtension))
                    {
                        return Json(new { success = false, message = "Invalid image type. Only JPG, JPEG, PNG, GIF, and WEBP files are allowed." });
                    }

                    // Create upload directory if it doesn't exist
                    var imageUploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "carousel-images");
                    if (!Directory.Exists(imageUploadsFolder))
                    {
                        Directory.CreateDirectory(imageUploadsFolder);
                    }

                    // Generate unique filename
                    var uniqueImageFileName = $"{Guid.NewGuid()}_{model.BackgroundImage.FileName}";
                    var imageFilePath = Path.Combine(imageUploadsFolder, uniqueImageFileName);

                    // Save file
                    using (var stream = new FileStream(imageFilePath, FileMode.Create))
                    {
                        await model.BackgroundImage.CopyToAsync(stream);
                    }

                    backgroundImageUrl = Path.Combine("uploads", "carousel-images", uniqueImageFileName).Replace("\\", "/");
                }

                // Handle Brochure upload (for Home page type only)
                if (model.PageType == CarouselPageType.Home && model.Brochure != null && model.Brochure.Length > 0)
                {
                    // Validate file size (10MB for PDFs)
                    if (model.Brochure.Length > 10 * 1024 * 1024)
                    {
                        return Json(new { success = false, message = "Brochure file size exceeds 10MB limit." });
                    }

                    // Validate PDF file extension
                    var brochureExtension = Path.GetExtension(model.Brochure.FileName).ToLowerInvariant();
                    if (brochureExtension != ".pdf")
                    {
                        return Json(new { success = false, message = "Brochure must be a PDF file." });
                    }

                    // Create upload directory if it doesn't exist
                    var brochureUploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "carousel-brochures");
                    if (!Directory.Exists(brochureUploadsFolder))
                    {
                        Directory.CreateDirectory(brochureUploadsFolder);
                    }

                    // Generate unique filename
                    var uniqueBrochureFileName = $"{Guid.NewGuid()}_{model.Brochure.FileName}";
                    var brochureFilePath = Path.Combine(brochureUploadsFolder, uniqueBrochureFileName);

                    // Save file
                    using (var stream = new FileStream(brochureFilePath, FileMode.Create))
                    {
                        await model.Brochure.CopyToAsync(stream);
                    }

                    brochureUrl = Path.Combine("uploads", "carousel-brochures", uniqueBrochureFileName).Replace("\\", "/");
                }

                // Call service to create carousel
                var result = await _carouselService.CreateCarouselService(model, backgroundImageUrl, brochureUrl);

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while creating the carousel. Please try again." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCarouselById(string id)
        {
            var result = await _carouselService.GetCarouselByIdService(id);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCarousel([FromForm] CarouselUpdateDto model)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(model.Id) || string.IsNullOrEmpty(model.Title) || string.IsNullOrEmpty(model.ButtonText))
                {
                    return Json(new { success = false, message = "ID, Title and Button Text are required." });
                }

                string backgroundImageUrl = null;
                string brochureUrl = null;

                // Handle Background Image upload (optional for update)
                if (model.BackgroundImage != null && model.BackgroundImage.Length > 0)
                {
                    // Validate file size (5MB for background images)
                    if (model.BackgroundImage.Length > 5 * 1024 * 1024)
                    {
                        return Json(new { success = false, message = "Background image size exceeds 5MB limit." });
                    }

                    // Validate image file extension
                    var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var imageExtension = Path.GetExtension(model.BackgroundImage.FileName).ToLowerInvariant();
                    if (!Array.Exists(allowedImageExtensions, ext => ext == imageExtension))
                    {
                        return Json(new { success = false, message = "Invalid image type. Only JPG, JPEG, PNG, GIF, and WEBP files are allowed." });
                    }

                    // Create upload directory if it doesn't exist
                    var imageUploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "carousel-images");
                    if (!Directory.Exists(imageUploadsFolder))
                    {
                        Directory.CreateDirectory(imageUploadsFolder);
                    }

                    // Generate unique filename
                    var uniqueImageFileName = $"{Guid.NewGuid()}_{model.BackgroundImage.FileName}";
                    var imageFilePath = Path.Combine(imageUploadsFolder, uniqueImageFileName);

                    // Save file
                    using (var stream = new FileStream(imageFilePath, FileMode.Create))
                    {
                        await model.BackgroundImage.CopyToAsync(stream);
                    }

                    backgroundImageUrl = Path.Combine("uploads", "carousel-images", uniqueImageFileName).Replace("\\", "/");
                }

                // Handle Brochure upload (optional for update, only for Home page type)
                if (model.PageType == CarouselPageType.Home && model.Brochure != null && model.Brochure.Length > 0)
                {
                    // Validate file size (10MB for PDFs)
                    if (model.Brochure.Length > 10 * 1024 * 1024)
                    {
                        return Json(new { success = false, message = "Brochure file size exceeds 10MB limit." });
                    }

                    // Validate PDF file extension
                    var brochureExtension = Path.GetExtension(model.Brochure.FileName).ToLowerInvariant();
                    if (brochureExtension != ".pdf")
                    {
                        return Json(new { success = false, message = "Brochure must be a PDF file." });
                    }

                    // Create upload directory if it doesn't exist
                    var brochureUploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "carousel-brochures");
                    if (!Directory.Exists(brochureUploadsFolder))
                    {
                        Directory.CreateDirectory(brochureUploadsFolder);
                    }

                    // Generate unique filename
                    var uniqueBrochureFileName = $"{Guid.NewGuid()}_{model.Brochure.FileName}";
                    var brochureFilePath = Path.Combine(brochureUploadsFolder, uniqueBrochureFileName);

                    // Save file
                    using (var stream = new FileStream(brochureFilePath, FileMode.Create))
                    {
                        await model.Brochure.CopyToAsync(stream);
                    }

                    brochureUrl = Path.Combine("uploads", "carousel-brochures", uniqueBrochureFileName).Replace("\\", "/");
                }

                // Call service to update carousel
                var result = await _carouselService.UpdateCarouselService(model, backgroundImageUrl, brochureUrl);

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while updating the carousel. Please try again." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCarousel(string id)
        {
            var result = await _carouselService.DeleteCarouselByIdService(id);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleCarouselStatus(string id)
        {
            var result = await _carouselService.ToggleCarouselStatusService(id);
            return Json(result);
        }
    }
}
