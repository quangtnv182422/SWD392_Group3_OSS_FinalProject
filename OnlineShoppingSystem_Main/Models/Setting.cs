using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineShoppingSystem_Main.Models;

public partial class Setting
{
    [Key]
    [StringLength(255)]
    [Unicode(false)]
    public string Id { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string SettingName { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string SettingValue { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? SettingCategoryId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? SettingStatusId { get; set; }

    [ForeignKey("SettingCategoryId")]
    [InverseProperty("Settings")]
    public virtual SettingCategory? SettingCategory { get; set; }

    [ForeignKey("SettingStatusId")]
    [InverseProperty("Settings")]
    public virtual SettingsStatus? SettingStatus { get; set; }
}
