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
        public async Task<IActionResult> Register(ClientRegistrationDto model, IFormFile? idFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (idFile != null && idFile.Length > 0)
            {
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "clients");
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);
                var ext = Path.GetExtension(idFile.FileName).ToLowerInvariant();
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsDir, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await idFile.CopyToAsync(stream);
                model.IDFilePath = $"/uploads/clients/{fileName}";
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
