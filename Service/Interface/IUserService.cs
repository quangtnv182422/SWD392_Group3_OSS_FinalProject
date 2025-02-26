using Microsoft.AspNetCore.Http;
using OnlineShoppingSystem_Main.Models;


namespace Service.Interface
{
    public interface IUserService
    {
        Task<string> GetUserIdAsync(HttpContext httpContext);
        AspNetUser GetCurrentUser(string userId);
    }
}
