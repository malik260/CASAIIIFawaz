using Core.DTOs;

namespace Logic.IServices
{
    public interface IMediaService
    {
        /// <summary>
        /// Saves a media record and updates the owner (Project/BuildingDesign) URL when purpose is Hero, Brochure, Image, or FloorPlan.
        /// </summary>
        Task<MediaDto?> SaveMediaAsync(string storedPath, string originalFileName, string contentType, long? fileSize, string purpose, string ownerType, string ownerId);
        List<MediaDto> GetByOwner(string ownerType, string ownerId);
        Task<MediaDto?> GetByIdAsync(string id);
        Task<bool> DeleteAsync(string id);
    }
}
