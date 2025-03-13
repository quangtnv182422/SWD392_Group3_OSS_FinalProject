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

namespace Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IUserRepository userRepository, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
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

        public async Task<IdentityUser> GetCurrentUserAsync(string userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }

        private async Task FakeLoginAsync(HttpContext httpContext)
        {
            var user = await _userManager.FindByIdAsync("User1");
            if (user == null)
            {
                user = new IdentityUser
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

        public async Task<IEnumerable<AspNetUser>> GetUsersAsync(string searchQuery)
        {
            var users = await _userRepository.GetUsersAsync(searchQuery);
            return users.Select(u => new AspNetUser
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber
            });
        }

        public async Task<AspNetUser> GetUserByIdAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return null;

            return new AspNetUser
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task<bool> AddUserAsync(AspNetUser user, string passwords)
        {
            var identityUser = new IdentityUser
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                PasswordHash = passwords
            };

            return await _userRepository.AddUserAsync(identityUser, "Default@123");
        }

        public async Task<bool> UpdateUserAsync(AspNetUser user)
        {
            var identityUser = new IdentityUser
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };

            return await _userRepository.UpdateUserAsync(identityUser);
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


        public async Task<IdentityUser> GetCurrentUserAsync()
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