﻿using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OnlineShoppingSystem_Main.Models;

namespace Service.Interface
{
    public interface IUserService
    {
        Task<string> GetUserIdAsync(HttpContext httpContext);
        Task<IdentityUser> GetCurrentUserAsync(string userId);
        Task<IEnumerable<AspNetUser>> GetUsersAsync(string searchQuery);
        Task<AspNetUser> GetUserByIdAsync(string userId);
        Task<bool> AddUserAsync(AspNetUser user, string passwords);
        Task<bool> UpdateUserAsync(AspNetUser user);
        Task<bool> DeleteUserAsync(string userId);
        Task<string> AutoCreatePasswords();
    }
}