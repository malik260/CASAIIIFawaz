using Core.DTOs;

namespace Logic.IServices
{
    public interface IStaffService
    {
        Task<Core.ViewModels.HeplerResponseVM> CreateStaffAsync(StaffCreateDto dto);
        List<TeamMemberDto> GetAllStaffAsTeamMembers();
        Task<TeamMemberDto?> GetStaffByIdAsync(string id);
        Task<Core.ViewModels.HeplerResponseVM> UpdateStaffAsync(string id, StaffCreateDto dto);
        Task<Core.ViewModels.HeplerResponseVM> DeleteStaffAsync(string id);
    }
}
