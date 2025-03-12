using Microsoft.AspNetCore.Identity;

namespace Repository.Interface
{
    public interface IUserRepository
    {
        Task<IdentityUser> GetUserByIdAsync(string userId);
        Task<IEnumerable<IdentityUser>> GetUsersAsync(string searchQuery);
        Task<bool> AddUserAsync(IdentityUser user, string password);
        Task<bool> UpdateUserAsync(IdentityUser user);
        Task<bool> DeleteUserAsync(string userId);
    }
}