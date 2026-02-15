using Core.DTOs;
using Logic.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using static Util;

namespace CASA3.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<Core.Model.AppUser> _userManager;

        public UserController(IUserService userService, UserManager<Core.Model.AppUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersService();
            var currentUser = Util.GetCurrentUser();
            if (currentUser == null || currentUser.Id == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var isSuperAdmin = currentUser.UserRole.Equals(Constants.SuperAdminRole);
            var isAdmin = currentUser.UserRole.Equals(Constants.AdminRole);
            var isStaff = currentUser.UserRole.Equals(Constants.StaffRole);

            // All authenticated users (SuperAdmin, Admin, Staff) can view
            ViewData["CanCreate"] = isSuperAdmin || isAdmin;
            ViewData["CanEdit"] = isSuperAdmin || isAdmin;
            ViewData["CanDelete"] = isSuperAdmin || isAdmin;
            ViewData["CanToggleStatus"] = isSuperAdmin || isAdmin;
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromForm] UserDto model)
        {
            try
            {
                // Check permission
                var currentUser = Util.GetCurrentUser();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "Unauthorized" });
                }

                var user = await _userManager.FindByIdAsync(currentUser.Id);
                if (user == null)
                {
                    return Json(new { success = false, message = "Current user not found" });
                }

                var roles = await _userManager.GetRolesAsync(user);
                var isSuperAdmin = roles.Contains(Constants.SuperAdminRole);
                var isAdmin = roles.Contains(Constants.AdminRole);

                if (!isSuperAdmin && !isAdmin)
                {
                    return Json(new { success = false, message = "You do not have permission to create users" });
                }

                // Validate required fields
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                {
                    return Json(new { success = false, message = "Email and Password are required." });
                }

                if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName))
                {
                    return Json(new { success = false, message = "First Name and Last Name are required." });
                }

                if (string.IsNullOrEmpty(model.Role) || 
                    (!model.Role.Equals(Constants.AdminRole, StringComparison.OrdinalIgnoreCase) && 
                     !model.Role.Equals(Constants.StaffRole, StringComparison.OrdinalIgnoreCase)))
                {
                    return Json(new { success = false, message = "Role must be either Admin or Staff." });
                }

                // Call service to create user
                var result = await _userService.CreateUserService(model);

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while creating the user. Please try again." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await _userService.GetUserByIdService(id);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser([FromForm] UserUpdateDto model)
        {
            try
            {
                // Check permission
                var currentUser = Util.GetCurrentUser();
                if (currentUser == null)
                {
                    return Json(new { success = false, message = "Unauthorized" });
                }

                var user = await _userManager.FindByIdAsync(currentUser.Id);
                if (user == null)
                {
                    return Json(new { success = false, message = "Current user not found" });
                }

                var roles = await _userManager.GetRolesAsync(user);
                var isSuperAdmin = roles.Contains(Constants.SuperAdminRole);
                var isAdmin = roles.Contains(Constants.AdminRole);

                if (!isSuperAdmin && !isAdmin)
                {
                    return Json(new { success = false, message = "You do not have permission to update users" });
                }

                // Validate required fields
                if (string.IsNullOrEmpty(model.Id) || string.IsNullOrEmpty(model.Email))
                {
                    return Json(new { success = false, message = "ID and Email are required." });
                }

                if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName))
                {
                    return Json(new { success = false, message = "First Name and Last Name are required." });
                }

                if (!string.IsNullOrEmpty(model.Role) &&
                    !model.Role.Equals(Constants.AdminRole, StringComparison.OrdinalIgnoreCase) &&
                    !model.Role.Equals(Constants.StaffRole, StringComparison.OrdinalIgnoreCase))
                {
                    return Json(new { success = false, message = "Role must be either Admin or Staff." });
                }

                // Call service to update user
                var result = await _userService.UpdateUserService(model);

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while updating the user. Please try again." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            // Check permission
            var currentUser = Util.GetCurrentUser();
            if (currentUser == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var user = await _userManager.FindByIdAsync(currentUser.Id);
            if (user == null)
            {
                return Json(new { success = false, message = "Current user not found" });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var isSuperAdmin = roles.Contains(Constants.SuperAdminRole);
            var isAdmin = roles.Contains(Constants.AdminRole);

            if (!isSuperAdmin && !isAdmin)
            {
                return Json(new { success = false, message = "You do not have permission to delete users" });
            }

            var result = await _userService.DeleteUserByIdService(id);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(string id)
        {
            // Check permission
            var currentUser = Util.GetCurrentUser();
            if (currentUser == null)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var user = await _userManager.FindByIdAsync(currentUser.Id);
            if (user == null)
            {
                return Json(new { success = false, message = "Current user not found" });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var isSuperAdmin = roles.Contains(Constants.SuperAdminRole);
            var isAdmin = roles.Contains(Constants.AdminRole);

            if (!isSuperAdmin && !isAdmin)
            {
                return Json(new { success = false, message = "You do not have permission to change user status" });
            }

            var result = await _userService.ToggleUserStatusService(id);
            return Json(result);
        }
    }
}

