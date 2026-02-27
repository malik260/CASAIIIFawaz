using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Logic.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CASA3.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration config)
        {
            var cloudName = config["Cloudinary:CloudName"]
                ?? throw new InvalidOperationException("Cloudinary:CloudName is not configured.");
            var apiKey = config["Cloudinary:ApiKey"]
                ?? throw new InvalidOperationException("Cloudinary:ApiKey is not configured.");
            var apiSecret = config["Cloudinary:ApiSecret"]
                ?? throw new InvalidOperationException("Cloudinary:ApiSecret is not configured.");

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account) { Api = { Secure = true } };
        }

        public async Task<string?> UploadImageAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0) return null;

            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                UseFilename = false,
                UniqueFilename = true,
                Overwrite = false
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            return result?.SecureUrl?.ToString();
        }

        public async Task<string?> UploadRawAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0) return null;

            await using var stream = file.OpenReadStream();
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                UseFilename = false,
                UniqueFilename = true,
                Overwrite = false
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            return result?.SecureUrl?.ToString();
        }

        public async Task DeleteAsync(string? cloudinaryUrl)
        {
            if (string.IsNullOrEmpty(cloudinaryUrl)) return;
            if (!cloudinaryUrl.Contains("cloudinary.com")) return;

            var publicId = ExtractPublicId(cloudinaryUrl, out var resourceType);
            if (string.IsNullOrEmpty(publicId)) return;

            var deleteParams = new DeletionParams(publicId) { ResourceType = resourceType };
            await _cloudinary.DestroyAsync(deleteParams);
        }

        /// <summary>
        /// Extracts the Cloudinary public_id from a secure URL.
        /// Example image URL:  https://res.cloudinary.com/cloud/image/upload/v123/folder/file.jpg  → folder/file
        /// Example raw URL:    https://res.cloudinary.com/cloud/raw/upload/v123/folder/file.pdf    → folder/file.pdf
        /// </summary>
        private static string? ExtractPublicId(string url, out ResourceType resourceType)
        {
            resourceType = ResourceType.Image;
            try
            {
                var uri = new Uri(url);
                var segments = uri.AbsolutePath.TrimStart('/').Split('/');

                // Detect resource type from URL path
                if (segments.Contains("raw"))
                    resourceType = ResourceType.Raw;
                else if (segments.Contains("video"))
                    resourceType = ResourceType.Video;

                // Find index of "upload" to know where the public_id begins
                var uploadIndex = Array.IndexOf(segments, "upload");
                if (uploadIndex < 0) return null;

                var startIndex = uploadIndex + 1;

                // Skip optional version segment (e.g., "v1234567890")
                if (startIndex < segments.Length
                    && segments[startIndex].StartsWith('v')
                    && long.TryParse(segments[startIndex][1..], out _))
                {
                    startIndex++;
                }

                var publicId = string.Join("/", segments[startIndex..]);

                // Strip extension for images and videos (Cloudinary public_id has no extension)
                if (resourceType != ResourceType.Raw)
                {
                    var lastDot = publicId.LastIndexOf('.');
                    if (lastDot > 0) publicId = publicId[..lastDot];
                }

                return string.IsNullOrEmpty(publicId) ? null : publicId;
            }
            catch
            {
                return null;
            }
        }
    }
}
