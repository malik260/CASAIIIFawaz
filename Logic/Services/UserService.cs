using Core.DB;
using Core.DTOs;
using Core.Model;
using Core.ViewModels;
using Logic.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using static Util;

namespace Logic.Services
{
    public class UserService : IUserService
    {
        private readonly ILoggerManager _log;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly EFContext _context;

        public UserService(ILoggerManager log, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, EFContext context)
        {
            _log = log;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<List<AppUserVM>> GetAllUsersService()
        {
            try
            {
                var users = await (
                    from user in _context.ApplicationUsers
                    join userRole in _context.UserRoles on user.Id equals userRole.UserId
                    join role in _context.Roles on userRole.RoleId equals role.Id
                    where role.Name != "SuperAdmin"
                    orderby user.DateRegistered descending
                    select new AppUserVM
                    {
                        Id = user.Id,
                        UserName = user.UserName ?? string.Empty,
                        FirstName = user.FirstName ?? string.Empty,
                        LastName = user.LastName ?? string.Empty,
                        Email = user.Email ?? string.Empty,
                        UserRole = role.Name,
                        DateRegistered = user.DateRegistered,
                        IsDeactivated = user.IsDeactivated,
                        DateModified = user.DateModified
                    }
                ).ToListAsync();

                return users;
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return new List<AppUserVM>();
            }
        }


        public async Task<HeplerResponseVM> CreateUserService(UserDto model)
        {
            var response = new HeplerResponseVM();
            try
            {
                var currentUser = Util.GetCurrentUser();
                if (currentUser == null)
                {
                    response.Message = "Unauthorized User";
                    return response;
                }

                // Check if current user has permission to create users
                var currentUserObj = await _userManager.FindByIdAsync(currentUser.Id);
                if (currentUserObj == null)
                {
                    response.Message = "Current user not found";
                    return response;
                }

                var currentUserRoles = await _userManager.GetRolesAsync(currentUserObj);
                var isSuperAdmin = currentUserRoles.Contains(Constants.SuperAdminRole);
                var isAdmin = currentUserRoles.Contains(Constants.AdminRole);

                // Only SuperAdmin and Admin can create users
                if (!isSuperAdmin && !isAdmin)
                {
                    response.Message = "You do not have permission to create users";
                    return response;
                }

                // Validate required fields
                if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                {
                    response.Message = "Email and Password are required";
                    return response;
                }

                // Validate role
                if (string.IsNullOrWhiteSpace(model.Role) || 
                    (!model.Role.Equals(Constants.AdminRole, StringComparison.OrdinalIgnoreCase) && 
                     !model.Role.Equals(Constants.StaffRole, StringComparison.OrdinalIgnoreCase)))
                {
                    response.Message = "Role must be either Admin or Staff";
                    return response;
                }

                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    response.Message = "A user with this email already exists";
                    return response;
                }

                // Create new user
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName?.Trim() ?? string.Empty,
                    LastName = model.LastName?.Trim() ?? string.Empty,
                    DateRegistered = DateTime.UtcNow,
                    EmailConfirmed = true,
                    IsDeactivated = false
                };

                var createResult = await _userManager.CreateAsync(user, model.Password);
                if (createResult.Succeeded)
                {
                    // Add user to role
                    var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
                    if (roleResult.Succeeded)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        var userRole = roles.FirstOrDefault() ?? "No Role";

                        response.success = true;
                        response.Message = "User Created Successfully";
                        response.Data = new AppUserVM
                        {
                            Id = user.Id,
                            UserName = user.UserName ?? string.Empty,
                            FirstName = user.FirstName ?? string.Empty,
                            LastName = user.LastName ?? string.Empty,
                            Email = user.Email ?? string.Empty,
                            UserRole = userRole,
                            DateRegistered = user.DateRegistered,
                            IsDeactivated = user.IsDeactivated,
                            DateModified = user.DateModified
                        };
                        return response;
                    }
                    else
                    {
                        // Rollback user creation if role assignment fails
                        await _userManager.DeleteAsync(user);
                        response.Message = "Failed to assign role to user";
                        return response;
                    }
                }
                else
                {
                    response.Message = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.success = false;
                response.Message = $"Failed with exception log";
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return response;
            }
        }

        public async Task<AppUserVM> GetUserByIdMain(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null || user.Email.Equals("SuperAdmin@casa3.com", StringComparison.OrdinalIgnoreCase))
                    return null;

                var roles = await _userManager.GetRolesAsync(user);
                var userRole = roles.FirstOrDefault() ?? "No Role";

                return new AppUserVM
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    UserRole = userRole,
                    DateRegistered = user.DateRegistered,
                    IsDeactivated = user.IsDeactivated,
                    DateModified = user.DateModified
                };
            }
            catch (Exception ex)
            {
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return null;
            }
        }

        public async Task<HeplerResponseVM> GetUserByIdService(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid Parameter Submitted";
                    return response;
                }

                var record = await GetUserByIdMain(id);
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
                response.Message = $"Failed with exception logged";
                _log.LogError(MethodBase.GetCurrentMethod()!, $"{ex?.Message} {ex?.InnerException?.Message}");
                return response;
            }
        }

        public async Task<HeplerResponseVM> UpdateUserService(UserUpdateDto model)
        {
            var response = new HeplerResponseVM();
            try
            {
                var currentUser = Util.GetCurrentUser();
                if (currentUser == null)
                {
                    response.Message = "Unauthorized User";
                    return response;
                }

                // Check if current user has permission to update users
                var currentUserObj = await _userManager.FindByIdAsync(currentUser.Id);
                if (currentUserObj == null)
                {
                    response.Message = "Current user not found";
                    return response;
                }

                var currentUserRoles = await _userManager.GetRolesAsync(currentUserObj);
                var isSuperAdmin = currentUserRoles.Contains(Constants.SuperAdminRole);
                var isAdmin = currentUserRoles.Contains(Constants.AdminRole);

                // Only SuperAdmin and Admin can update users
                if (!isSuperAdmin && !isAdmin)
                {
                    response.Message = "You do not have permission to update users";
                    return response;
                }

                if (model == null || string.IsNullOrEmpty(model.Id) || string.IsNullOrEmpty(model.Email))
                {
                    response.Message = "Invalid Parameter Submitted";
                    return response;
                }

                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null || user.Email.Equals("SuperAdmin@casa3.com", StringComparison.OrdinalIgnoreCase))
                {
                    response.Message = "No Record Found";
                    return response;
                }

                // Validate role if provided
                if (!string.IsNullOrWhiteSpace(model.Role) &&
                    !model.Role.Equals(Constants.AdminRole, StringComparison.OrdinalIgnoreCase) &&
                    !model.Role.Equals(Constants.StaffRole, StringComparison.OrdinalIgnoreCase))
                {
                    response.Message = "Role must be either Admin or Staff";
                    return response;
                }

                // Check if email is being changed and if new email already exists
                if (!user.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var existingUser = await _userManager.FindByEmailAsync(model.Email);
                    if (existingUser != null && existingUser.Id != model.Id)
                    {
                        response.Message = "A user with this email already exists";
                        return response;
                    }
                }

                // Update user properties
                user.FirstName = model.FirstName?.Trim() ?? string.Empty;
                user.LastName = model.LastName?.Trim() ?? string.Empty;
                user.Email = model.Email.Trim();
                user.UserName = model.Email.Trim();
                user.DateModified = DateTime.UtcNow;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    response.Message = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                    return response;
                }

                // Update password if provided
                if (!string.IsNullOrWhiteSpace(model.NewPassword))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                    if (!passwordResult.Succeeded)
                    {
                        response.Message = "User updated but password change failed: " + string.Join(", ", passwordResult.Errors.Select(e => e.Description));
                        return response;
                    }
                }

                // Update role if provided
                if (!string.IsNullOrWhiteSpace(model.Role))
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    if (currentRoles.Any())
                    {
                        await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    }
                    await _userManager.AddToRoleAsync(user, model.Role);
                }

                response.success = true;
                response.Message = "Updated Successfully";
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

        public async Task<HeplerResponseVM> DeleteUserByIdService(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                var currentUser = Util.GetCurrentUser();
                if (currentUser == null)
                {
                    response.Message = "Unauthorized User";
                    return response;
                }

                // Check if current user has permission to delete users
                var currentUserObj = await _userManager.FindByIdAsync(currentUser.Id);
                if (currentUserObj == null)
                {
                    response.Message = "Current user not found";
                    return response;
                }

                var currentUserRoles = await _userManager.GetRolesAsync(currentUserObj);
                var isSuperAdmin = currentUserRoles.Contains(Constants.SuperAdminRole);
                var isAdmin = currentUserRoles.Contains(Constants.AdminRole);

                // Only SuperAdmin and Admin can delete users
                if (!isSuperAdmin && !isAdmin)
                {
                    response.Message = "You do not have permission to delete users";
                    return response;
                }

                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid Parameter Submitted";
                    return response;
                }

                var user = await _userManager.FindByIdAsync(id);
                if (user == null || user.Email.Equals("SuperAdmin@casa3.com", StringComparison.OrdinalIgnoreCase))
                {
                    response.Message = "No Record Found or Cannot delete SuperAdmin";
                    return response;
                }

                // Prevent deleting yourself
                if (user.Id == currentUser.Id)
                {
                    response.Message = "You cannot delete your own account";
                    return response;
                }

                var deleteResult = await _userManager.DeleteAsync(user);
                if (deleteResult.Succeeded)
                {
                    response.success = true;
                    response.Message = "Deleted Successfully";
                }
                else
                {
                    response.success = false;
                    response.Message = string.Join(", ", deleteResult.Errors.Select(e => e.Description));
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

        public async Task<HeplerResponseVM> ToggleUserStatusService(string id)
        {
            var response = new HeplerResponseVM();
            try
            {
                var currentUser = Util.GetCurrentUser();
                if (currentUser == null)
                {
                    response.Message = "Unauthorized User";
                    return response;
                }

                // Check if current user has permission
                var currentUserObj = await _userManager.FindByIdAsync(currentUser.Id);
                if (currentUserObj == null)
                {
                    response.Message = "Current user not found";
                    return response;
                }

                var currentUserRoles = await _userManager.GetRolesAsync(currentUserObj);
                var isSuperAdmin = currentUserRoles.Contains(Constants.SuperAdminRole);
                var isAdmin = currentUserRoles.Contains(Constants.AdminRole);

                // Only SuperAdmin and Admin can toggle user status
                if (!isSuperAdmin && !isAdmin)
                {
                    response.Message = "You do not have permission to change user status";
                    return response;
                }

                if (string.IsNullOrEmpty(id))
                {
                    response.Message = "Invalid Parameter Submitted";
                    return response;
                }

                var user = await _userManager.FindByIdAsync(id);
                if (user == null || user.Email.Equals("SuperAdmin@casa3.com", StringComparison.OrdinalIgnoreCase))
                {
                    response.Message = "No Record Found or Cannot modify SuperAdmin";
                    return response;
                }

                // Prevent deactivating yourself
                if (user.Id == currentUser.Id)
                {
                    response.Message = "You cannot deactivate your own account";
                    return response;
                }

                user.IsDeactivated = !user.IsDeactivated;
                user.DateModified = DateTime.UtcNow;

                var updateResult = await _userManager.UpdateAsync(user);
                if (updateResult.Succeeded)
                {
                    response.success = true;
                    response.Message = $"User {(user.IsDeactivated ? "Deactivated" : "Activated")} Successfully";
                    response.Data = user.IsDeactivated;
                }
                else
                {
                    response.success = false;
                    response.Message = string.Join(", ", updateResult.Errors.Select(e => e.Description));
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

