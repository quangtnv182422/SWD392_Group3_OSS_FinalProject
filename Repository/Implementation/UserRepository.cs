using Microsoft.AspNetCore.Identity;
using OnlineShoppingSystem_Main.Models;
using Repository.Interface;
using System.Diagnostics;

namespace Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserRepository(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityUser> GetUserByIdAsync(string userId)
        {
            Debug.WriteLine($"[DEBUG] Looking for user with ID: {userId}");
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                Debug.WriteLine("[DEBUG] User not found in database");
            }
            else
            {
                Debug.WriteLine($"[DEBUG] Found user in database: Id={user.Id}, UserName={user.UserName}");
            }

            return user;
        }
    }
}