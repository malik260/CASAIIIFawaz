using Core.DB;
using Core.DTOs;
using Core.Model;
using Core.ViewModels;
using Logic.IServices;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Logic.Services
{
    public class StaffService : IStaffService
    {
        private readonly EFContext _context;
        private readonly ILoggerManager _log;

        public StaffService(EFContext context, ILoggerManager log)
        {
            _context = context;
            _log = log;
        }

        public async Task<HeplerResponseVM> CreateStaffAsync(StaffCreateDto dto)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Position))
                {
                    response.Message = "Name and Position are required.";
                    return response;
                }

                var staff = new Staff
                {
                    Name = dto.Name,
                    Position = dto.Position,
                    ImageUrl = dto.ImageUrl,
                    Address = dto.Address,
                    PhoneNumber = dto.PhoneNumber,
                    LinkedInUrl = dto.LinkedInUrl,
                    MemberInfo = dto.MemberInfo,
                    Category = dto.Category
                };

                await _context.Staffs.AddAsync(staff).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                response.success = true;
                response.Message = "Staff added successfully.";
                response.Data = MapToDto(staff);
                return response;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                response.Message = "An error occurred while adding staff.";
                return response;
            }
        }

        public List<TeamMemberDto> GetAllStaffAsTeamMembers()
        {
            try
            {
                var list = _context.Staffs
                    .Where(s => !s.IsDeleted)
                    .OrderBy(s => s.Name)
                    .ToList()
                    .Select(MapToDto)
                    .ToList();
                return list;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return new List<TeamMemberDto>();
            }
        }

        public async Task<TeamMemberDto?> GetStaffByIdAsync(string id)
        {
            try
            {
                var staff = await _context.Staffs
                    .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted)
                    .ConfigureAwait(false);
                return staff == null ? null : MapToDto(staff);
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return null;
            }
        }

        public async Task<HeplerResponseVM> UpdateStaffAsync(string id, StaffCreateDto dto)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id) || dto == null)
                {
                    response.Message = "Invalid parameters.";
                    return response;
                }

                var staff = await _context.Staffs
                    .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted)
                    .ConfigureAwait(false);
                if (staff == null)
                {
                    response.Message = "Staff not found.";
                    return response;
                }

                staff.Name = dto.Name;
                staff.Position = dto.Position;
                staff.ImageUrl = dto.ImageUrl;
                staff.Address = dto.Address;
                staff.PhoneNumber = dto.PhoneNumber;
                staff.LinkedInUrl = dto.LinkedInUrl;
                staff.MemberInfo = dto.MemberInfo;
                staff.Category = dto.Category;
                staff.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync().ConfigureAwait(false);
                response.success = true;
                response.Message = "Staff updated successfully.";
                response.Data = MapToDto(staff);
                return response;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                response.Message = "An error occurred while updating staff.";
                return response;
            }
        }

        public async Task<HeplerResponseVM> DeleteStaffAsync(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid parameter.";
                    return response;
                }

                var staff = await _context.Staffs
                    .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted)
                    .ConfigureAwait(false);
                if (staff == null)
                {
                    response.Message = "Staff not found.";
                    return response;
                }

                staff.IsDeleted = true;
                staff.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync().ConfigureAwait(false);
                response.success = true;
                response.Message = "Staff removed successfully.";
                return response;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                response.Message = "An error occurred while removing staff.";
                return response;
            }
        }

        private static TeamMemberDto MapToDto(Staff s)
        {
            return new TeamMemberDto
            {
                Id = s.Id,
                Name = s.Name,
                Position = s.Position,
                ImageUrl = s.ImageUrl ?? string.Empty,
                Address = s.Address,
                PhoneNumber = s.PhoneNumber,
                LinkedInUrl = s.LinkedInUrl ?? string.Empty,
                MemberInfo = s.MemberInfo ?? string.Empty,
                Category = s.Category
            };
        }
    }
}
