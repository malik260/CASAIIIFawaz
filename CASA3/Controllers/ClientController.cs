using Core.DTOs;
using Logic.IServices;
using Microsoft.AspNetCore.Mvc;

namespace CASA3.Controllers
{
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IWebHostEnvironment _env;

        public ClientController(IClientService clientService, IWebHostEnvironment env)
        {
            _clientService = clientService;
            _env = env;
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewData["Title"] = "Client Registration";
            return View(new ClientRegistrationDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(ClientRegistrationDto model, IFormFile? idFile, IFormFile? passportFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "clients");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            if (idFile != null && idFile.Length > 0)
            {
                var ext = Path.GetExtension(idFile.FileName).ToLowerInvariant();
                var fileName = $"id-{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsDir, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await idFile.CopyToAsync(stream);
                model.IDFilePath = $"/uploads/clients/{fileName}";
            }

            if (passportFile != null && passportFile.Length > 0)
            {
                var ext = Path.GetExtension(passportFile.FileName).ToLowerInvariant();
                var fileName = $"passport-{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsDir, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await passportFile.CopyToAsync(stream);
                model.PassportFilePath = $"/uploads/clients/{fileName}";
            }

            var result = await _clientService.CreateClientService(model);
            if (result.success)
            {
                TempData["ClientSuccess"] = result.Message;
                return RedirectToAction(nameof(Success));
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        public IActionResult Success()
        {
            ViewData["Title"] = "Registration Successful";
            return View();
        }
    }
}
