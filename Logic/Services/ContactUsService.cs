using Core.DB;
using Core.DTOs;
using Core.Enum;
using Core.Model;
using Core.Models;
using Core.ViewModels;
using Logic.IServices;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Logic.Services
{
    public class ContactUsService : IContactUsService
    {
        private readonly ILoggerManager _log;
        private readonly EFContext _context;

        public ContactUsService(EFContext context, ILoggerManager log)
        {
            _context = context;
            _log = log;
        }

        public List<ContactUsVM> GetAllContactUsService()
        {
            try
            {
                var contacts = _context.Contacts.Where(a => !a.IsDeleted)
                    .ToList()                   // materialize first
                    .Select(MapContactUsToVM)      // now mapping works
                    .ToList();

                if (contacts.Count == 0)
                {
                    _log.Loginfo(MethodBase.GetCurrentMethod()!, "No contact found.");
                    return new List<ContactUsVM>();
                }

                return contacts;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return new List<ContactUsVM>();
            }
        }

        public async Task<HeplerResponseVM> CreateContactUsService(ContactFormDto registration)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (registration != null)
                {
                    if (!string.IsNullOrEmpty(registration.FirstName) && !string.IsNullOrEmpty(registration.Email) && !string.IsNullOrEmpty(registration.Phone))
                    {
                        var contact = new Contact()
                        {
                            FirstName = registration?.FirstName!,
                            LastName = registration?.LastName!,
                            Email = registration?.Email!,
                            Phone = registration.Phone,
                            Request = registration.Request,
                            Budget = registration.Budget,
                            Message = registration.Message,
                        };
                        await _context.AddAsync(contact).ConfigureAwait(false);
                        await _context.SaveChangesAsync();
                        response.success = true ;
                        response.Message = "Thank you for contacting us. We'll be in touch shortly";
                        response.Data = MapContactUsToVM(contact);
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

        public async Task<ContactUsVM> GetContactUsIdMain(string id)
        {
            try
            {
                var model = await _context.Contacts.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
                if (model != null)
                {
                    return MapContactUsToVM(model);
                }
                return null;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return null;
            }

        }
        public async Task<HeplerResponseVM> GetContactUsByIdService(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid Parameter Submitted"; return response;
                }
                var record = await GetContactUsIdMain(id);
                if (record != null)
                {
                    response.success = true ; response.Message = "Successful"; response.Data = record;
                }
                else
                {
                    response.success = false; response.Message = "No Record Found";
                }
                return response;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.Message = $"Failed with exception logged";
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return response;
            }
        }

        public async Task<HeplerResponseVM> MarkAsContactedService(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid Parameter Submitted"; return response;
                }
                var rex = await _context.Contacts.Where(v => v.Id == id).ExecuteUpdateAsync(setters => setters
                                       .SetProperty(v => v.Status, ContactFormStatus.Contacted)
                                       .SetProperty(v => v.UpdatedAt, DateTime.Now));
                if (rex > 0)
                {
                    response.success = true ;
                    response.Message = "Marked Successful";
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

        public async Task<HeplerResponseVM> DeleteContactUsByIdService(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid Parameter Submitted"; return response;
                }
                var rex = await _context.Contacts.Where(v => v.Id == id).ExecuteUpdateAsync(setters => setters
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

        public ContactUsVM MapContactUsToVM(Contact contact)
        {
            if (contact == null) return null;
            return new ContactUsVM
            {
                Id = contact.Id,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Email = contact.Email,
                Phone = contact.Phone,
                Request = contact.Request,
                Budget = contact.Budget,
                Message = contact.Message,
                CreatedAt = contact.CreatedAt,
                UpdatedAt = contact.UpdatedAt,
                Status = contact.Status
            };
        }
    }
}
