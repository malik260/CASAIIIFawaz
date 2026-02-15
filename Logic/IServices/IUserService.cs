using Core.DTOs;
using Core.ViewModels;

namespace Logic.IServices
{
    public interface IUserService
    {
        Task<List<AppUserVM>> GetAllUsersService();
        Task<HeplerResponseVM> CreateUserService(UserDto model);
        Task<AppUserVM> GetUserByIdMain(string id);
        Task<HeplerResponseVM> GetUserByIdService(string id);
        Task<HeplerResponseVM> UpdateUserService(UserUpdateDto model);
        Task<HeplerResponseVM> DeleteUserByIdService(string id);
        Task<HeplerResponseVM> ToggleUserStatusService(string id);
    }
}

