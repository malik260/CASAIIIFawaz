using Core.DTOs;
using Core.ViewModels;

namespace Logic.IServices
{
    public interface IContactUsService
    {
        Task<HeplerResponseVM> CreateContactUsService(ContactFormDto registration);
        Task<HeplerResponseVM> DeleteContactUsByIdService(string id);
        List<ContactUsVM> GetAllContactUsService();
        Task<HeplerResponseVM> GetContactUsByIdService(string id);
        Task<ContactUsVM> GetContactUsIdMain(string id);
        Task<HeplerResponseVM> MarkAsContactedService(string id);
    }
}
