using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using OnlineShoppingSystem_Main.Models;
using Repository.Interface;
using Service.Interface;
using System.Diagnostics;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Data.Models;
using System.Text;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;

namespace Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RoleManager<AspNetRole> _roleManager;

        public UserService(IUserRepository userRepository, UserManager<AspNetUser> userManager, IHttpContextAccessor httpContextAccessor, RoleManager<AspNetRole> roleManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _roleManager = roleManager;
        }

        public async Task<string> GetUserIdAsync(HttpContext httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                await FakeLoginAsync(httpContext);
            }

            string userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Debug.WriteLine($"[DEBUG] User ID from Claims: {userId}");
            return userId ?? "User1";
        }

        public async Task<AspNetUser> GetCurrentUserAsync(string userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }

        private async Task FakeLoginAsync(HttpContext httpContext)
        {
            var user = await _userManager.FindByIdAsync("User1");
            if (user == null)
            {
                user = new AspNetUser
                {
                    Id = "User1",
                    UserName = "User1",
                    Email = "user1@example.com",
                    PhoneNumber = "1234567890"
                };
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create fake user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
                Debug.WriteLine("[DEBUG] Created fake user 'User1' in database.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "User1"),
                new Claim(ClaimTypes.Name, "User1"),
                new Claim(ClaimTypes.Email, "user1@example.com")
            };

            var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
            var principal = new ClaimsPrincipal(identity);

            await httpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);
            Debug.WriteLine("[DEBUG] Fake login for User1 completed.");
        }

        public async Task<IEnumerable<AspNetUser>> GetUsersAsync(string searchQuery, string roleFilter, string statusFilter)
        {
            return await _userRepository.GetUsersAsync(searchQuery, roleFilter, statusFilter);
        }

        public async Task<List<string>> GetUserRolesAsync(AspNetUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }

        public async Task<AspNetUser> GetUserByIdAsync(string userId)
        {
            Debug.WriteLine($"[DEBUG] Looking for user with ID: {userId}");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                Debug.WriteLine("[DEBUG] User not found in database");
                return null;
            }

            Debug.WriteLine($"[DEBUG] Found user: Id={user.Id}, UserName={user.UserName}");

            // Lấy danh sách role của user
            var roles = await _userManager.GetRolesAsync(user);
            Debug.WriteLine($"[DEBUG] User roles: {string.Join(", ", roles)}");

            return new AspNetUser
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                LockoutEnabled = user.LockoutEnabled,
                Roles = roles.Select(roleName => new AspNetRole { Name = roleName }).ToList() // Gán roles vào user
            };
        }

        public async Task<AspNetUser> GetUserForUpdateByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user;
        }


        public async Task<bool> AddUserAsync(AspNetUser user, string password, string role)
        {
            // Tạo user bằng UserManager (chuẩn Identity)
            var createResult = await _userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                // Log lỗi để biết lỗi gì
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                Console.WriteLine($"[DEBUG] Create user failed: {errors}");
                return false;
            }
            // Kiểm tra nếu role không rỗng, thì gán role
            if (!string.IsNullOrEmpty(role))
            {
                var roleExists = await _roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    var roleCreateResult = await _roleManager.CreateAsync(new AspNetRole { Name = role, NormalizedName = role.ToUpper() });
            if (!roleCreateResult.Succeeded)
            {
                var errors = string.Join(", ", roleCreateResult.Errors.Select(e => e.Description));
                Console.WriteLine($"[DEBUG] Create role '{role}' failed: {errors}");
                return false;
            }
                }

                // Gán user vào role
                var addToRoleResult = await _userManager.AddToRoleAsync(user, role);
                if (!addToRoleResult.Succeeded)
                {
                    var errors = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description));
                    Console.WriteLine($"[DEBUG] Add user to role '{role}' failed: {errors}");
                    return false;
                }
            }

            return true;
        }

        public async Task<List<string>> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            return roles;
        }

        public async Task<bool> UpdateUserAsync(AspNetUser userInput)
        {
            var result = await _userManager.UpdateAsync(userInput);
            return result.Succeeded;
        }

        public async Task<bool> UpdateUserRolesAsync(AspNetUser user, List<string> newRoles)
        {
            // Lấy roles hiện tại
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Xoá roles cũ mà không có trong newRoles
            var rolesToRemove = currentRoles.Where(r => !newRoles.Contains(r)).ToList();
            if (rolesToRemove.Any())
                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            // Thêm roles mới
            var rolesToAdd = newRoles.Where(r => !currentRoles.Contains(r)).ToList();
            if (rolesToAdd.Any())
                await _userManager.AddToRolesAsync(user, rolesToAdd);

            return true;
        }


        public async Task<bool> DeleteUserAsync(string userId)
        {
            return await _userRepository.DeleteUserAsync(userId);
        }

        public async Task<string> AutoCreatePasswords()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            Random random = new Random();

            string password = new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return await Task.FromResult(password);
        }


        public async Task<AspNetUser> GetCurrentUserAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                return await _userManager.GetUserAsync(user);
            }
            return null;
        }

        public async Task<string> GetCurrentUserIdAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                return user.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            return null; 
        }
    }
}