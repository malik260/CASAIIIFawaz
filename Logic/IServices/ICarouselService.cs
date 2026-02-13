using Core.DTOs;
using Core.Enum;
using Core.ViewModels;

namespace Logic.IServices
{
    public interface ICarouselService
    {
        List<CarouselVM> GetAllCarouselsService();
        List<CarouselVM> GetCarouselsByPageTypeService(CarouselPageType pageType);
        Task<HeplerResponseVM> CreateCarouselService(CarouselDto model, string backgroundImageUrl, string? brochureUrl);
        Task<CarouselVM> GetCarouselByIdMain(string id);
        Task<HeplerResponseVM> GetCarouselByIdService(string id);
        Task<HeplerResponseVM> UpdateCarouselService(CarouselUpdateDto model, string? backgroundImageUrl, string? brochureUrl);
        Task<HeplerResponseVM> DeleteCarouselByIdService(string id);
        Task<HeplerResponseVM> ToggleCarouselStatusService(string id);
    }
}
