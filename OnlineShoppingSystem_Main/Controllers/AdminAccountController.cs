using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Service.Interface;
using System.Threading.Tasks;
using Data.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace OnlineShoppingSystem_Main.Controllers
{
    public class AdminAccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailService _emailService;

        public AdminAccountController(IUserService userService, UserManager<IdentityUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        // Hiển thị danh sách user với tìm kiếm
        public async Task<IActionResult> AccountList(string searchQuery, string roleFilter, string statusFilter, int page = 1, int pageSize = 10)
        {
            var users = await _userService.GetUsersAsync(searchQuery);

            if (!string.IsNullOrEmpty(roleFilter))
            {
                users = users.Where(u => u.Roles.Any(r => r.Name == roleFilter));
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                users = users.Where(u => u.LockoutEnabled == (statusFilter == "Deactivated"));
            }


            int totalUsers = users.Count();
            var pagedUsers = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.SearchQuery = searchQuery;
            ViewBag.RoleFilter = roleFilter;
            ViewBag.StatusFilter = statusFilter;

            return View(pagedUsers);
        }


        // Xem chi tiết user
        public async Task<IActionResult> ViewUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new { message = "User ID is required." });
            }

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return View("AccountDetail", user);
        }

        // Thêm user mới
        [HttpPost]
        public async Task<IActionResult> AddUser(string username, string email)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
            {
                return Json(new { success = false, message = "Username, Email, and Password are required." });
            }

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return Json(new { success = false, message = "User with this email already exists." });
            }

            string password = await _userService.AutoCreatePasswords();

            var user = new AspNetUser { UserName = username, Email = email };
            bool result = await _userService.AddUserAsync(user, password);

            if (result)
            {
                await _emailService.SendWelcomeEmail(email, username, password);
                return Json(new { success = true, message = "User added successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Failed to add user." });
            }
        }

        // Xóa user
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "User ID is required." });
            }

            bool result = await _userService.DeleteUserAsync(id);

            if (result)
            {
                return Json(new { success = true, message = "User deleted successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Failed to delete user. User may not exist." });
            }
        }

    }
}
