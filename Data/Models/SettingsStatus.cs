using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineShoppingSystem_Main.Models;

[Table("SettingsStatus")]
public partial class SettingsStatus
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string StatusName { get; set; } = null!;

    [InverseProperty("SettingStatus")]
    public virtual ICollection<Setting> Settings { get; set; } = new List<Setting>();
}
