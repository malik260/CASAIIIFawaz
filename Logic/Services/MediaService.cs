using Core.DB;
using Core.DTOs;
using Core.Model;
using Logic.IServices;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Logic.Services
{
    public class MediaService : IMediaService
    {
        private readonly EFContext _context;
        private readonly ILoggerManager _log;

        public MediaService(EFContext context, ILoggerManager log)
        {
            _context = context;
            _log = log;
        }

        public async Task<MediaDto?> SaveMediaAsync(string storedPath, string originalFileName, string contentType, long? fileSize, string purpose, string ownerType, string ownerId)
        {
            try
            {
                var media = new Media
                {
                    StoredPath = storedPath,
                    OriginalFileName = originalFileName,
                    ContentType = contentType,
                    FileSize = fileSize,
                    Purpose = purpose,
                    OwnerType = ownerType,
                    OwnerId = ownerId
                };
                await _context.Media.AddAsync(media).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);

                await UpdateOwnerUrlAsync(ownerType, ownerId, purpose, storedPath).ConfigureAwait(false);

                return MapToDto(media);
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return null;
            }
        }

        private async Task UpdateOwnerUrlAsync(string ownerType, string ownerId, string purpose, string storedPath)
        {
            try
            {
                if (ownerType == "Project")
                {
                    var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == ownerId && !p.IsDeleted).ConfigureAwait(false);
                    if (project != null)
                    {
                        if (string.Equals(purpose, "Hero", StringComparison.OrdinalIgnoreCase))
                            project.HeroImageUrl = storedPath;
                        else if (string.Equals(purpose, "Brochure", StringComparison.OrdinalIgnoreCase))
                            project.BrochurePdfUrl = storedPath;
                        project.UpdatedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync().ConfigureAwait(false);
                    }
                }
                else if (ownerType == "BuildingDesign")
                {
                    var design = await _context.BuildingDesigns.FirstOrDefaultAsync(b => b.Id == ownerId && !b.IsDeleted).ConfigureAwait(false);
                    if (design != null)
                    {
                        if (string.Equals(purpose, "Image", StringComparison.OrdinalIgnoreCase))
                            design.ImageUrl = storedPath;
                        else if (string.Equals(purpose, "Brochure", StringComparison.OrdinalIgnoreCase))
                            design.BrochurePdfUrl = storedPath;
                        else if (string.Equals(purpose, "FloorPlan", StringComparison.OrdinalIgnoreCase))
                            design.FloorPlanPdfUrl = storedPath;
                        design.UpdatedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
            }
        }

        public List<MediaDto> GetByOwner(string ownerType, string ownerId)
        {
            try
            {
                return _context.Media
                    .Where(m => m.OwnerType == ownerType && m.OwnerId == ownerId && !m.IsDeleted)
                    .OrderByDescending(m => m.CreatedAt)
                    .ToList()
                    .Select(MapToDto)
                    .ToList();
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return new List<MediaDto>();
            }
        }

        public async Task<MediaDto?> GetByIdAsync(string id)
        {
            try
            {
                var media = await _context.Media.FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted).ConfigureAwait(false);
                return media == null ? null : MapToDto(media);
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                var media = await _context.Media.FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted).ConfigureAwait(false);
                if (media == null) return false;
                media.IsDeleted = true;
                media.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return false;
            }
        }

        private static MediaDto MapToDto(Media m)
        {
            return new MediaDto
            {
                Id = m.Id,
                StoredPath = m.StoredPath,
                OriginalFileName = m.OriginalFileName,
                ContentType = m.ContentType,
                FileSize = m.FileSize,
                Purpose = m.Purpose,
                OwnerType = m.OwnerType,
                OwnerId = m.OwnerId
            };
        }
    }
}
