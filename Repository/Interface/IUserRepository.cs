using Data.Models;
using Microsoft.AspNetCore.Identity;
using OnlineShoppingSystem_Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IUserRepository
    {
        AspNetUser GetUserById(string userId);
        /*Task<IdentityUser> GetUserById(string userId);*/
    }
}