using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    public class AspNetRoleClaim : IdentityRoleClaim<string>
    {
        [ForeignKey("RoleId")]
        [InverseProperty("AspNetRoleClaims")]
        public virtual AspNetRole Role { get; set; } = null!;
    }
}
