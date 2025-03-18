using Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Repository.Interface
{
    public interface IUserRepository
    {
        Task<AspNetUser> GetUserByIdAsync(string userId);
        Task<IEnumerable<AspNetUser>> GetUsersAsync(string searchQuery, string roleFilter, string statusFilter);
        Task<bool> AddUserAsync(AspNetUser user, string password);
        Task<bool> UpdateUserAsync(AspNetUser user);
        Task<bool> DeleteUserAsync(string userId);
    }
}