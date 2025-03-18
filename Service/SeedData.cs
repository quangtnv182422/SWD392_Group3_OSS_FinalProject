using Data.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

public static class SeedData
{
    public static async Task SeedRolesAsync(RoleManager<AspNetRole> roleManager)
    {
        var roleNames = new[] { "admin", "sales", "customer" };

        foreach (var roleName in roleNames)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                var role = new AspNetRole
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                };
                await roleManager.CreateAsync(role);
            }
        }
    }
}
