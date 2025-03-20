using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Service.Interface;
using System.Threading.Tasks;
using Data.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authorization;

namespace OnlineShoppingSystem_Main.Controllers
{
    //[Authorize(Roles = "admin")]
    public class AdminAccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IEmailService _emailService;

        public AdminAccountController(IUserService userService, UserManager<AspNetUser> userManager, IEmailService emailService)
        {
            _userService = userService;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<IActionResult> AccountList(string searchQuery, string roleFilter, string statusFilter, int page = 1, int pageSize = 10)
        {
            var users = await _userService.GetUsersAsync(searchQuery, roleFilter, statusFilter);

            int totalUsers = users.Count();
            var pagedUsers = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Lấy danh sách Roles cho từng User
            var userRoles = new Dictionary<string, List<string>>();
            foreach (var user in pagedUsers)
            {
                var roles = await _userService.GetUserRolesAsync(user);
                userRoles[user.Id] = roles.ToList();
            }

            // Truyền roles vào ViewBag
            ViewBag.UserRoles = userRoles;

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

            // Lấy danh sách role của user
            var roles = await _userService.GetUserRolesAsync(user);
            ViewBag.UserRoles = roles; // Truyền role của user vào ViewBag

            // Lấy tất cả roles có trong hệ thống
            var allRoles = await _userService.GetAllRolesAsync();
            ViewBag.AllRoles = allRoles; // Truyền tất cả roles vào ViewBag

            return View("AccountDetail", user);
        }

        // Thêm user mới
        [HttpPost]
        public async Task<IActionResult> AddUser(string username, string email, string phoneNumber, DateTime DOB, string address, string role)
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

            var user = new AspNetUser { 
                UserName = username, 
                Email = email,
                PhoneNumber = phoneNumber,
                DateOfBirth = DOB != null && DOB >= new DateTime(1753, 1, 1) ? DOB : null,
                Address = address,
            };

            bool result = await _userService.AddUserAsync(user, password, role);

            if (result)
            {
                await _emailService.SendWelcomeEmail(email, username, password);
                return RedirectToAction("AccountList");
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
                return RedirectToAction("AccountList");
            }
            else
            {
                return Json(new { success = false, message = "Failed to delete user. User may not exist." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(AspNetUser userInput, List<string> newRoles)
        {
            var existingUser = await _userService.GetUserForUpdateByIdAsync(userInput.Id);
            if (existingUser == null)
            {
                return NotFound("User not found.");
            }

            existingUser.UserName = userInput.UserName;
            existingUser.Email = userInput.Email;
            existingUser.PhoneNumber = userInput.PhoneNumber;
            existingUser.Address = userInput.Address;
            existingUser.DateOfBirth = userInput.DateOfBirth;
            existingUser.LockoutEnabled = userInput.LockoutEnabled;

            // Gọi hàm update user
            var isUserUpdated = await _userService.UpdateUserAsync(existingUser);

            Console.WriteLine($"[DEBUG] New roles: {string.Join(", ", newRoles)}");

            // Cập nhật role dựa trên "existingUser" (cùng 1 instance)
            var isRolesUpdated = await _userService.UpdateUserRolesAsync(existingUser, newRoles);
            if (!isRolesUpdated)
                return BadRequest("Failed to update user roles.");

            if (isUserUpdated)
                return RedirectToAction("ViewUser", new { id = existingUser.Id });
            else
                return BadRequest(new { success = false, message = "Failed to update user." });
        }

    }
}
