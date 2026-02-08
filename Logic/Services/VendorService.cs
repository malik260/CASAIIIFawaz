using Core.DB;
using Core.DTOs;
using Core.Model;
using Core.ViewModels;
using Logic.IServices;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Logic.Services
{
    public class VendorService : IVendorService
    {

        private readonly ILoggerManager _log;
        private readonly EFContext _context;

        public VendorService(EFContext context, ILoggerManager log)
        {
            _context = context;
            _log = log;
        }

        public List<VendorVM> GetAllRegisteredVendorsService()
        {
            try
            {
                var vendors = _context.Vendors.Where(a => !a.IsDeleted)
                    .ToList()                   // materialize first
                    .Select(MapVendorToVM)      // now mapping works
                    .ToList();

                if (vendors.Count == 0)
                {
                    _log.Loginfo(MethodBase.GetCurrentMethod()!, "No vendors found.");
                    return new List<VendorVM>();
                }

                return vendors;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return new List<VendorVM>();
            }
        }

        public async Task<HeplerResponseVM> CreateVendorService(VendorRegistrationDto registration)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (registration != null)
                {
                    if (!string.IsNullOrEmpty(registration.CompanyName) && !string.IsNullOrEmpty(registration.ContactPerson) && !string.IsNullOrEmpty(registration.Email)
                    && !string.IsNullOrEmpty(registration.PhoneNumber) && !string.IsNullOrEmpty(registration.CACNumber) && !string.IsNullOrEmpty(registration.TIN))
                    {
                        var checkForTel = _context.Vendors.Any(u => u.PhoneNumber == registration.PhoneNumber);
                        if (checkForTel)
                        {
                            response.Message = "Phone Already Exist"; return response;
                        }
                        var checkForEmail = _context.Vendors.Any(u => u.Email == registration.Email);
                        if (checkForEmail)
                        {
                            response.Message = "Email Already Exist"; return response;
                        }
                        var checkForTIN = _context.Vendors.Any(u => u.TIN == registration.TIN);
                        if (checkForEmail)
                        {
                            response.Message = "TIN Already Exist"; return response;
                        }
                        var checkForCAC = _context.Vendors.Any(u => u.CACNumber == registration.CACNumber);
                        if (checkForEmail)
                        {
                            response.Message = "CAC Number Already Exist"; return response;
                        }

                        var vendor = new Vendor()
                        {
                            CompanyName = registration?.CompanyName!,
                            ContactPerson = registration?.ContactPerson!,
                            Email = registration?.Email!,
                            PhoneNumber = registration.PhoneNumber,
                            CACNumber = registration.CACNumber,
                            TIN = registration.TIN,
                            BusinessCategory = registration.BusinessCategory,
                            BusinessAddress = registration.BusinessAddress,
                            FilePath = registration.FilePath,
                        };
                        await _context.AddAsync(vendor).ConfigureAwait(false);
                        await _context.SaveChangesAsync();
                        response.success = true ;
                        response.Message = "Registered Successfully";
                        response.Data = MapVendorToVM(vendor);
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

        public async Task<VendorVM> GetVendorIdMain(string id)
        {
            try
            {
                var model = await _context.Vendors.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
                if (model != null)
                {
                    return MapVendorToVM(model);
                }
                return null;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return null;
            }

        }
        public async Task<HeplerResponseVM> GetVendorByIdService(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid Parameter Submitted"; return response;
                }
                var record = await GetVendorIdMain(id);
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

        public async Task<HeplerResponseVM> UpdateStudentService(VendorUpdateDto model)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (!string.IsNullOrEmpty(model.Id) && !string.IsNullOrEmpty(model.CompanyName) && !string.IsNullOrEmpty(model.ContactPerson) && !string.IsNullOrEmpty(model.Email) 
                    && !string.IsNullOrEmpty(model.PhoneNumber) && !string.IsNullOrEmpty(model.CACNumber) && !string.IsNullOrEmpty(model.TIN))
                {
                    var checkForTel = _context.Vendors.Any(u => u.PhoneNumber == model.PhoneNumber && u.Id != model.Id);
                    if (checkForTel)
                    {
                        response.Message = "Phone Already Exist"; return response;
                    }
                    var checkForEmail = _context.Vendors.Any(u => u.Email == model.Email && u.Id != model.Id);
                    if (checkForEmail)
                    {
                        response.Message = "Email Already Exist"; return response;
                    }
                    var checkForTIN = _context.Vendors.Any(u => u.TIN == model.TIN && u.Id != model.Id);
                    if (checkForEmail)
                    {
                        response.Message = "TIN Already Exist"; return response;
                    }
                    var checkForCAC = _context.Vendors.Any(u => u.CACNumber == model.CACNumber && u.Id != model.Id);
                    if (checkForEmail)
                    {
                        response.Message = "CAC Number Already Exist"; return response;
                    }
                    // Build the update conditionally
                    var query = _context.Vendors.Where(v => v.Id == model.Id);

                    var setters = query.ExecuteUpdateAsync(s => s
                        .SetProperty(v => v.CompanyName, model.CompanyName)
                        .SetProperty(v => v.ContactPerson, model.ContactPerson)
                        .SetProperty(v => v.Email, model.Email)
                        .SetProperty(v => v.PhoneNumber, model.PhoneNumber)
                        .SetProperty(v => v.CACNumber, model.CACNumber)
                        .SetProperty(v => v.TIN, model.TIN)
                        .SetProperty(v => v.BusinessCategory, model.BusinessCategory)
                        .SetProperty(v => v.BusinessAddress, model.BusinessAddress)
                        .SetProperty(v => v.Status, model.Status)
                        .SetProperty(v => v.UpdatedAt, DateTime.UtcNow));

                    // Only update FilesPath if a new value is provided
                    if (!string.IsNullOrEmpty(model.FilePath))
                    {
                        await _context.Vendors
                            .Where(v => v.Id == model.Id)
                            .ExecuteUpdateAsync(s => s.SetProperty(v => v.FilePath, model.FilePath));
                    }

                    await setters;
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

        public async Task<HeplerResponseVM> DeleteVendorByIdService(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid Parameter Submitted"; return response;
                }
                var rex = await _context.Vendors.Where(v => v.Id == id).ExecuteUpdateAsync(setters => setters
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


        private VendorVM MapVendorToVM(Vendor vendor)
        {
            if (vendor == null) return null;
            return new VendorVM
            {
                Id = vendor.Id,
                CompanyName = vendor.CompanyName,
                ContactPerson = vendor.ContactPerson,
                Email = vendor.Email,
                PhoneNumber = vendor.PhoneNumber,
                CACNumber = vendor.CACNumber,
                TIN = vendor.TIN,
                BusinessCategory = vendor.BusinessCategory,
                BusinessAddress = vendor.BusinessAddress,
                CreatedAt = vendor.CreatedAt,
                UpdatedAt = vendor.UpdatedAt,
                Status = vendor.Status
            };
        }
    }
}
