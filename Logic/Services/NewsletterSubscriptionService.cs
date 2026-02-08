using Core.DB;
using Core.DTOs;
using Core.Model;
using Core.Models;
using Core.ViewModels;
using Logic.IServices;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Logic.Services
{
    public class NewsletterSubscriptionService : INewsletterSubscriptionService
    {
        private readonly ILoggerManager _log;
        private readonly EFContext _context;

        public NewsletterSubscriptionService(EFContext context, ILoggerManager log)
        {
            _context = context;
            _log = log;
        }

        public List<NewsletterSubscriptionVM> GetAllNewsletterSubscriptionsService()
        {
            try
            {
                var nl = _context.NewsletterSubscriptions.Where(a => !a.IsDeleted).Select(a => new NewsletterSubscriptionVM()
                {
                    Id = a.Id,
                    Email = a.Email,
                    CreatedAt = a.CreatedAt,
                }).ToList();

                if (nl.Count == 0)
                {
                    _log.Loginfo(MethodBase.GetCurrentMethod()!, "No news letter subscription found.");
                    return new List<NewsletterSubscriptionVM>();
                }
                return nl;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return new List<NewsletterSubscriptionVM>();
            }
        }

        public async Task<HeplerResponseVM> CreateNewsletterSubscriptionsService(NewsletterSubscriptionDto sub)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (sub != null)
                {
                    if (!string.IsNullOrEmpty(sub.Email))
                    {
                        var checkForEmail = _context.NewsletterSubscriptions.Any(u => u.Email == sub.Email);
                        if (checkForEmail)
                        {
                            response.Message = "Email already subscribed to our news letter"; return response;
                        }
                        var nl = new NewsletterSubscription()
                        {
                            Email = sub?.Email!,
                        };
                        await _context.AddAsync(nl).ConfigureAwait(false);
                        await _context.SaveChangesAsync();
                        response.success = true ;
                        response.Message = "Created Successfully";
                        response.Data = new NewsletterSubscriptionVM() { Email = nl.Email, CreatedAt = nl.CreatedAt, Id = nl.Id };
                        return response;
                    }
                }
                response.Message = "Invalid Parameter Submitted"; return response;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.Message = $"Failed with exception log";
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return response;
            }
        }

        public async Task<HeplerResponseVM> DeleteNewsletterSubscriptionsByIdService(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid Parameter Submitted"; return response;
                }
                var rex = await _context.NewsletterSubscriptions.Where(v => v.Id == id).ExecuteUpdateAsync(setters => setters
                                       .SetProperty(v => v.IsDeleted, true)
                                       .SetProperty(v => v.UpdatedAt, DateTime.Now));
                if (rex > 0)
                {
                    response.success = true ;
                    response.Message = "Deleted Successful";
                }
                else
                {
                    response.success = false;
                    response.Message = "No Record Found";
                }
                return response;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.Message = $"Failed with exception log";
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return response;
            }
        }

    }
}
