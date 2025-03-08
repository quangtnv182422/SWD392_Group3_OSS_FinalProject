using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OnlineShoppingSystem_Main.Models;


namespace Service.Interface
{
    public interface IUserService
    {
        Task<string> GetUserIdAsync(HttpContext httpContext);
        AspNetUser GetCurrentUser(string userId);
        /*Task<IdentityUser>GetCurrentUser(string userId);*/
    }
}