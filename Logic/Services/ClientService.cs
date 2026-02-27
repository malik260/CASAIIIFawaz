using Core.DB;
using Core.DTOs;
using Core.Model;
using Core.ViewModels;
using Logic.IServices;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Logic.Services
{
    public class ClientService : IClientService
    {
        private readonly EFContext _context;
        private readonly ILoggerManager _log;

        public ClientService(EFContext context, ILoggerManager log)
        {
            _context = context;
            _log = log;
        }

        public List<ClientVM> GetAllClientsService()
        {
            try
            {
                var clients = _context.Clients
                    .Where(c => !c.IsDeleted)
                    .ToList()
                    .Select(MapClientToVM)
                    .ToList();

                return clients;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return new List<ClientVM>();
            }
        }

        public async Task<HeplerResponseVM> CreateClientService(ClientRegistrationDto registration)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (registration == null)
                {
                    response.Message = "Invalid Parameter Submitted";
                    return response;
                }

                if (string.IsNullOrWhiteSpace(registration.FullName) ||
                    string.IsNullOrWhiteSpace(registration.Email) ||
                    string.IsNullOrWhiteSpace(registration.PhoneNumber) ||
                    string.IsNullOrWhiteSpace(registration.AddressLine1) ||
                    string.IsNullOrWhiteSpace(registration.State) ||
                    string.IsNullOrWhiteSpace(registration.City) ||
                    string.IsNullOrWhiteSpace(registration.NextOfKinFirstName) ||
                    string.IsNullOrWhiteSpace(registration.NextOfKinLastName) ||
                    string.IsNullOrWhiteSpace(registration.NextOfKinPhoneNumber) ||
                    string.IsNullOrWhiteSpace(registration.NextOfKinResidentialAddress))
                {
                    response.Message = "Please fill all required fields";
                    return response;
                }

                if (_context.Clients.Any(c => c.Email == registration.Email && !c.IsDeleted))
                {
                    response.Message = "A client with this email already exists";
                    return response;
                }

                if (_context.Clients.Any(c => c.PhoneNumber == registration.PhoneNumber && !c.IsDeleted))
                {
                    response.Message = "A client with this phone number already exists";
                    return response;
                }

                var client = new Client
                {
                    ClientType = registration.ClientType,
                    FullName = registration.FullName,
                    PhoneNumber = registration.PhoneNumber,
                    Email = registration.Email,
                    AddressType = registration.AddressType,
                    AddressLine1 = registration.AddressLine1,
                    AddressLine2 = registration.AddressLine2,
                    Country = registration.Country ?? "Nigeria",
                    State = registration.State,
                    City = registration.City,
                    DateOfBirthOrIncorporation = registration.DateOfBirthOrIncorporation,
                    MeansOfID = registration.MeansOfID,
                    IDFilePath = registration.IDFilePath,
                    NextOfKinFirstName = registration.NextOfKinFirstName,
                    NextOfKinLastName = registration.NextOfKinLastName,
                    NextOfKinOtherNames = registration.NextOfKinOtherNames,
                    NextOfKinPhoneNumber = registration.NextOfKinPhoneNumber,
                    NextOfKinEmail = registration.NextOfKinEmail,
                    NextOfKinRelationship = registration.NextOfKinRelationship,
                    NextOfKinResidentialAddress = registration.NextOfKinResidentialAddress,
                    NextOfKinOfficeAddress = registration.NextOfKinOfficeAddress,
                };

                await _context.Clients.AddAsync(client);
                await _context.SaveChangesAsync();

                response.success = true;
                response.Message = "Client registered successfully";
                response.Data = MapClientToVM(client);
                return response;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.Message = "Registration failed. Please try again.";
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return response;
            }
        }

        public async Task<ClientVM> GetClientByIdMain(string id)
        {
            try
            {
                var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
                return client != null ? MapClientToVM(client) : null;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return null;
            }
        }

        public async Task<HeplerResponseVM> GetClientByIdService(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid Parameter Submitted";
                    return response;
                }
                var record = await GetClientByIdMain(id);
                if (record != null)
                {
                    response.success = true;
                    response.Message = "Successful";
                    response.Data = record;
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
                response.Message = "Failed with exception logged";
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return response;
            }
        }

        public async Task<HeplerResponseVM> UpdateClientService(ClientUpdateDto model)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(model.Id) ||
                    string.IsNullOrWhiteSpace(model.FullName) ||
                    string.IsNullOrWhiteSpace(model.Email) ||
                    string.IsNullOrWhiteSpace(model.PhoneNumber))
                {
                    response.Message = "Invalid Parameter Submitted";
                    return response;
                }

                if (_context.Clients.Any(c => c.Email == model.Email && c.Id != model.Id && !c.IsDeleted))
                {
                    response.Message = "Email already in use by another client";
                    return response;
                }

                if (_context.Clients.Any(c => c.PhoneNumber == model.PhoneNumber && c.Id != model.Id && !c.IsDeleted))
                {
                    response.Message = "Phone number already in use by another client";
                    return response;
                }

                await _context.Clients.Where(c => c.Id == model.Id).ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.ClientType, model.ClientType)
                    .SetProperty(c => c.FullName, model.FullName)
                    .SetProperty(c => c.PhoneNumber, model.PhoneNumber)
                    .SetProperty(c => c.Email, model.Email)
                    .SetProperty(c => c.AddressType, model.AddressType)
                    .SetProperty(c => c.AddressLine1, model.AddressLine1)
                    .SetProperty(c => c.AddressLine2, model.AddressLine2)
                    .SetProperty(c => c.Country, model.Country ?? "Nigeria")
                    .SetProperty(c => c.State, model.State)
                    .SetProperty(c => c.City, model.City)
                    .SetProperty(c => c.DateOfBirthOrIncorporation, model.DateOfBirthOrIncorporation)
                    .SetProperty(c => c.MeansOfID, model.MeansOfID)
                    .SetProperty(c => c.NextOfKinFirstName, model.NextOfKinFirstName)
                    .SetProperty(c => c.NextOfKinLastName, model.NextOfKinLastName)
                    .SetProperty(c => c.NextOfKinOtherNames, model.NextOfKinOtherNames)
                    .SetProperty(c => c.NextOfKinPhoneNumber, model.NextOfKinPhoneNumber)
                    .SetProperty(c => c.NextOfKinEmail, model.NextOfKinEmail)
                    .SetProperty(c => c.NextOfKinRelationship, model.NextOfKinRelationship)
                    .SetProperty(c => c.NextOfKinResidentialAddress, model.NextOfKinResidentialAddress)
                    .SetProperty(c => c.NextOfKinOfficeAddress, model.NextOfKinOfficeAddress)
                    .SetProperty(c => c.Status, model.Status)
                    .SetProperty(c => c.Notes, model.Notes)
                    .SetProperty(c => c.UpdatedAt, DateTime.UtcNow));

                if (!string.IsNullOrEmpty(model.IDFilePath))
                {
                    await _context.Clients.Where(c => c.Id == model.Id)
                        .ExecuteUpdateAsync(s => s.SetProperty(c => c.IDFilePath, model.IDFilePath));
                }

                response.success = true;
                response.Message = "Client updated successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.Message = "Update failed. Please try again.";
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return response;
            }
        }

        public async Task<HeplerResponseVM> DeleteClientByIdService(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid Parameter Submitted";
                    return response;
                }

                var rows = await _context.Clients.Where(c => c.Id == id).ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.IsDeleted, true)
                    .SetProperty(c => c.UpdatedAt, DateTime.UtcNow));

                if (rows > 0)
                {
                    response.success = true;
                    response.Message = "Client deleted successfully";
                }
                else
                {
                    response.success = false;
                    response.Message = "No record found";
                }
                return response;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.Message = "Delete failed. Please try again.";
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return response;
            }
        }

        private static ClientVM MapClientToVM(Client client)
        {
            if (client == null) return null;
            return new ClientVM
            {
                Id = client.Id,
                ClientType = client.ClientType,
                FullName = client.FullName,
                PhoneNumber = client.PhoneNumber,
                Email = client.Email,
                AddressType = client.AddressType,
                AddressLine1 = client.AddressLine1,
                AddressLine2 = client.AddressLine2,
                Country = client.Country,
                State = client.State,
                City = client.City,
                DateOfBirthOrIncorporation = client.DateOfBirthOrIncorporation,
                MeansOfID = client.MeansOfID,
                IDFilePath = client.IDFilePath,
                NextOfKinFirstName = client.NextOfKinFirstName,
                NextOfKinLastName = client.NextOfKinLastName,
                NextOfKinOtherNames = client.NextOfKinOtherNames,
                NextOfKinPhoneNumber = client.NextOfKinPhoneNumber,
                NextOfKinEmail = client.NextOfKinEmail,
                NextOfKinRelationship = client.NextOfKinRelationship,
                NextOfKinResidentialAddress = client.NextOfKinResidentialAddress,
                NextOfKinOfficeAddress = client.NextOfKinOfficeAddress,
                Status = client.Status,
                Notes = client.Notes,
                CreatedAt = client.CreatedAt,
                UpdatedAt = client.UpdatedAt,
            };
        }
    }
}
