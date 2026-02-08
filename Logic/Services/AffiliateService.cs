using Core.DB;
using Core.DTOs;
using Core.Model;
using Core.ViewModels;
using Logic.IServices;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Logic.Services
{
    public class AffiliateService : IAffiliateService
    {

        private readonly ILoggerManager _log;
        private readonly EFContext _context;

        public AffiliateService(EFContext context, ILoggerManager log)
        {
            _context = context;
            _log = log;
        }

        public List<AffiliateVM> GetAllRegisteredAffiliatesService()
        {
            try
            {
                var affiliates = _context.Affiliates.Where(a => !a.IsDeleted)
                    .ToList()                   // materialize first
                    .Select(MapAffiliateToVM)      // now mapping works
                    .ToList();

                if (affiliates.Count == 0)
                {
                    _log.Loginfo(MethodBase.GetCurrentMethod()!, "No affiliate found.");
                    return new List<AffiliateVM>();
                }

                return affiliates;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return new List<AffiliateVM>();
            }
        }

        public async Task<HeplerResponseVM> CreateAffiliateService(AffiliateRegistrationDto registration)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (registration != null)
                {
                    if (!string.IsNullOrEmpty(registration.FirstName) && !string.IsNullOrEmpty(registration.LastName) && !string.IsNullOrEmpty(registration.Email) && !string.IsNullOrEmpty(registration.Phone))
                    {
                        var checkForTel = _context.Affiliates.Any(u => u.Phone == registration.Phone);
                        if (checkForTel)
                        {
                            response.Message = "Phone Already Exist"; return response;
                        }
                        var checkForEmail = _context.Affiliates.Any(u => u.Email == registration.Email);
                        if (checkForEmail)
                        {
                            response.Message = "Email Already Exist"; return response;
                        }
                        var aff = new Affiliate()
                        {
                            FirstName = registration?.FirstName!,
                            LastName = registration?.LastName!,
                            Email = registration?.Email!,
                            Phone = registration.Phone,
                            StreetAddress = registration.StreetAddress,
                            StateProvince = registration.StateProvince,
                            Country = registration.Country,
                            AccountName = registration.AccountName,
                            BankName = registration.BankName,
                            AccountNumber = registration.AccountNumber,
                        };
                        await _context.AddAsync(aff).ConfigureAwait(false);
                        await _context.SaveChangesAsync();
                        response.success = true ;
                        response.Message = "Registered Successfully";
                        response.Data = MapAffiliateToVM(aff);
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

        public async Task<AffiliateVM> GetAffiliateIdMain(string id)
        {
            try
            {
                var model = await _context.Affiliates.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
                if (model != null)
                {
                    return MapAffiliateToVM(model);
                }
                return null;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return null;
            }

        }
        public async Task<HeplerResponseVM> GetAffiliateByIdService(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid Parameter Submitted"; return response;
                }
                var record = await GetAffiliateIdMain(id);
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

        public async Task<HeplerResponseVM> UpdateStudentService(AffiliateUpdateDto model)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (!string.IsNullOrEmpty(model.Id) && !string.IsNullOrEmpty(model.FirstName) && !string.IsNullOrEmpty(model.LastName) && !string.IsNullOrEmpty(model.Email) && !string.IsNullOrEmpty(model.Phone))
                {
                    var checkForTel = _context.Affiliates.Any(u => u.Phone == model.Phone && u.Id != model.Id);
                    if (checkForTel)
                    {
                        response.Message = "Phone Already Exist"; return response;
                    }
                    var checkForEmail = _context.Affiliates.Any(u => u.Email == model.Email && u.Id != model.Id);
                    if (checkForEmail)
                    {
                        response.Message = "Email Already Exist"; return response;
                    }
                    await _context.Affiliates.Where(v => v.Id == model.Id).ExecuteUpdateAsync(setters => setters
                                           .SetProperty(v => v.FirstName, model.FirstName)
                                           .SetProperty(v => v.LastName, model.LastName)
                                           .SetProperty(v => v.Phone, model.Phone)
                                           .SetProperty(v => v.StreetAddress, model.StreetAddress)
                                           .SetProperty(v => v.StateProvince, model.StateProvince)
                                           .SetProperty(v => v.Country, model.Country)
                                           .SetProperty(v => v.AccountName, model.AccountName)
                                           .SetProperty(v => v.BankName, model.BankName)
                                           .SetProperty(v => v.AccountNumber, model.AccountNumber)
                                           .SetProperty(v => v.Status, model.Status)
                                           .SetProperty(v => v.UpdatedAt, DateTime.Now));
                    response.success = true ;
                    response.Message = "Updated Successfully";
                    return response;
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

        public async Task<HeplerResponseVM> DeleteAffiliateByIdService(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid Parameter Submitted"; return response;
                }
                var rex = await _context.Affiliates.Where(v => v.Id == id).ExecuteUpdateAsync(setters => setters
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


        public AffiliateVM MapAffiliateToVM(Affiliate affiliate)
        {
            if (affiliate == null) return null;
            return new AffiliateVM
            {
                Id = affiliate.Id,
                FirstName = affiliate.FirstName,
                LastName = affiliate.LastName,
                Email = affiliate.Email,
                Phone = affiliate.Phone,
                StreetAddress = affiliate.StreetAddress,
                StateProvince = affiliate.StateProvince,
                Country = affiliate.Country,
                AccountName = affiliate.AccountName,
                BankName = affiliate.BankName,
                AccountNumber = affiliate.AccountNumber,
                CreatedAt = affiliate.CreatedAt,
                UpdatedAt = affiliate.UpdatedAt,
                Status = affiliate.Status
            };
        }
    }
}
