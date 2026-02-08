using Core.DTOs;
using Core.ViewModels;

namespace Logic.IServices
{
    public interface INewsletterSubscriptionService
    {
        Task<HeplerResponseVM> CreateNewsletterSubscriptionsService(NewsletterSubscriptionDto sub);
        Task<HeplerResponseVM> DeleteNewsletterSubscriptionsByIdService(string id);
        List<NewsletterSubscriptionVM> GetAllNewsletterSubscriptionsService();
    }
}
