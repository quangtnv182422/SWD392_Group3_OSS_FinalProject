using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using OnlineShoppingSystem_Main.Models;
using Repository.Interface;
using Service.Interface;
using System.Diagnostics;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;


namespace Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<string> GetUserIdAsync(HttpContext httpContext)
        {
            await FakeLoginAsync(httpContext);
            string userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Debug.WriteLine($"[DEBUG] User ID from Claims: {userId}");
            return userId ?? "User1";
        }

        public AspNetUser GetCurrentUser(string userId)
        {
            return _userRepository.GetUserById(userId);
        }

        private async Task FakeLoginAsync(HttpContext httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, "User1"),
                    new Claim(ClaimTypes.Name, "User1"),
                    new Claim(ClaimTypes.Email, "user1@example.com")
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                Debug.WriteLine("[DEBUG] Fake login for User1 completed.");
            }
        }
    }
}
