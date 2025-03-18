using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingSystem_Main.Models;
using Repository.Interface;
using System.Diagnostics;

namespace Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AspNetUser> _userManager;

        public UserRepository(UserManager<AspNetUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<AspNetUser> GetUserByIdAsync(string userId)
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
        public async Task<IEnumerable<AspNetUser>> GetUsersAsync(string searchQuery, string roleFilter, string statusFilter)
        {
            var query = _userManager.Users.AsQueryable();

            // Lọc theo từ khoá tìm kiếm
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(u => u.UserName.Contains(searchQuery) || u.Email.Contains(searchQuery));
            }

            // Lọc theo Status (Active/Deactivated)
            if (!string.IsNullOrEmpty(statusFilter))
            {
                bool isDeactivated = statusFilter == "Deactivated";
                query = query.Where(u => u.LockoutEnabled == isDeactivated);
            }

            var users = await query.ToListAsync();

            // Nếu có lọc Role, phải gọi GetRolesAsync để kiểm tra user nào có role đó
            if (!string.IsNullOrEmpty(roleFilter))
            {
                var filteredUsers = new List<AspNetUser>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains(roleFilter))
                    {
                        filteredUsers.Add(user);
                    }
                }
                return filteredUsers;
            }

            return users;
        }

        public async Task<bool> AddUserAsync(AspNetUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                Debug.WriteLine($"[DEBUG] Failed to add user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            return result.Succeeded;
        }

        public async Task<bool> UpdateUserAsync(AspNetUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                Debug.WriteLine($"[DEBUG] Failed to add user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
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