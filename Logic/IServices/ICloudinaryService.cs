using Microsoft.AspNetCore.Http;

namespace Logic.IServices
{
    public interface ICloudinaryService
    {
        /// <summary>Upload an image file to Cloudinary and return its secure URL.</summary>
        Task<string?> UploadImageAsync(IFormFile file, string folder);

        /// <summary>Upload a raw file (PDF, DOC, etc.) to Cloudinary and return its secure URL.</summary>
        Task<string?> UploadRawAsync(IFormFile file, string folder);

        /// <summary>Delete a previously uploaded file from Cloudinary using its stored URL.</summary>
        Task DeleteAsync(string? cloudinaryUrl);
    }
}
