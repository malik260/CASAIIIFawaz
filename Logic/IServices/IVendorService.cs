using Core.DTOs;
using Core.ViewModels;

namespace Logic.IServices
{
    public interface IVendorService
    {
        Task<HeplerResponseVM> CreateVendorService(VendorRegistrationDto registration);
        Task<HeplerResponseVM> DeleteVendorByIdService(string id);
        List<VendorVM> GetAllRegisteredVendorsService();
        Task<HeplerResponseVM> GetVendorByIdService(string id);
        Task<VendorVM> GetVendorIdMain(string id);
        Task<HeplerResponseVM> UpdateStudentService(VendorUpdateDto model);
    }
}
