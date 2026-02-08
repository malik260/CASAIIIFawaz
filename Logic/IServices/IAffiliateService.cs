using Core.DTOs;
using Core.ViewModels;

namespace Logic.IServices
{
    public interface IAffiliateService
    {
        Task<HeplerResponseVM> CreateAffiliateService(AffiliateRegistrationDto registration);
        Task<HeplerResponseVM> DeleteAffiliateByIdService(string id);
        Task<HeplerResponseVM> GetAffiliateByIdService(string id);
        Task<AffiliateVM> GetAffiliateIdMain(string id);
        List<AffiliateVM> GetAllRegisteredAffiliatesService();
        Task<HeplerResponseVM> UpdateStudentService(AffiliateUpdateDto model);
    }
}
