using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using OnlineShoppingSystem_Main.Models;
using Repository.Interface;
using Service.Interface;
using System.Diagnostics;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public UserService(IUserRepository userRepository, UserManager<IdentityUser> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
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
    }
}