using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OnlineShoppingSystem_Main.Models;

namespace Service.Interface
{
    public interface IUserService
    {
        Task<string> GetUserIdAsync(HttpContext httpContext);
        Task<AspNetUser> GetCurrentUserAsync(string userId);
        Task<IEnumerable<AspNetUser>> GetUsersAsync(string searchQuery, string roleFilter, string statusFilter);
        Task<AspNetUser> GetUserByIdAsync(string userId);
        Task<bool> AddUserAsync(AspNetUser user, string passwords, string role);
        Task<bool> UpdateUserAsync(AspNetUser user);
        Task<bool> UpdateUserRolesAsync(AspNetUser user, List<string> newRoles);
        Task<bool> DeleteUserAsync(string userId);
        Task<string> AutoCreatePasswords();

        // Phần này Quang đang dùng cho xét điều kiện Login hay chưa để lấu infor tạo order sau khi thanh toán
        Task<AspNetUser> GetCurrentUserAsync();
        Task<string> GetCurrentUserIdAsync();
        Task<List<string>> GetUserRolesAsync(AspNetUser user);
        Task<List<string>> GetAllRolesAsync();
        Task<AspNetUser> GetUserForUpdateByIdAsync(string userId);
    }
}