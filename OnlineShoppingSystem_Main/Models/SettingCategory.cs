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
    [StringLength(255)]
    [Unicode(false)]
    public string Id { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string CategoryName { get; set; } = null!;

    [InverseProperty("SettingCategory")]
    public virtual ICollection<Setting> Settings { get; set; } = new List<Setting>();
}
