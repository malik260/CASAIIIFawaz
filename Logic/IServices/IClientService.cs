using Core.DTOs;
using Core.ViewModels;

namespace Logic.IServices
{
    public interface IClientService
    {
        Task<HeplerResponseVM> CreateClientService(ClientRegistrationDto registration);
        Task<HeplerResponseVM> UpdateClientService(ClientUpdateDto model);
        Task<HeplerResponseVM> DeleteClientByIdService(string id);
        List<ClientVM> GetAllClientsService();
        Task<ClientVM> GetClientByIdMain(string id);
        Task<HeplerResponseVM> GetClientByIdService(string id);
    }
}
