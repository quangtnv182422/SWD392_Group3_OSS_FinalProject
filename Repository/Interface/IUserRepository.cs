using Microsoft.AspNetCore.Identity;

namespace Repository.Interface
{
    public interface IUserRepository
    {
        Task<IdentityUser> GetUserByIdAsync(string userId);
    }
}