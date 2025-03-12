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
        public async Task<IEnumerable<IdentityUser>> GetUsersAsync(string searchQuery)
        {
            var users = _userManager.Users.AsQueryable();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                users = users.Where(u => u.UserName.Contains(searchQuery) || u.Email.Contains(searchQuery));
            }
            return await Task.FromResult(users.ToList());
        }

        public async Task<bool> AddUserAsync(IdentityUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                Debug.WriteLine($"[DEBUG] Failed to add user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            return result.Succeeded;
        }

        public async Task<bool> UpdateUserAsync(IdentityUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

    }
}