using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineShoppingSystem_Main.Models;

[Table("SettingCategory")]
public partial class SettingCategory
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string CategoryName { get; set; } = null!;

    [InverseProperty("SettingCategory")]
    public virtual ICollection<Setting> Settings { get; set; } = new List<Setting>();
}
