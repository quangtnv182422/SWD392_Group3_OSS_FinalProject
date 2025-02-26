using Microsoft.EntityFrameworkCore;
using OnlineShoppingSystem_Main.Data.Models;
using OnlineShoppingSystem_Main.Models;
using Repository.Interface;
using System.Diagnostics;


namespace Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly Swd392OssContext _context;

        public UserRepository(Swd392OssContext context)
        {
            _context = context;
        }
        public AspNetUser GetUserById(string userId)
        {
            Debug.WriteLine($"[DEBUG] Looking for user with ID: {userId}");

            // Use explicit mapping and disable tracking
            var user = _context.AspNetUsers
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new AspNetUser
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber
                    // Add other properties you need
                })
                .FirstOrDefault();

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