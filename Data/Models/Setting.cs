using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Setting
{
    public int Id { get; set; }

    public string SettingName { get; set; } = null!;

    public string SettingValue { get; set; } = null!;

    public int? SettingCategoryId { get; set; }

    public int? SettingStatusId { get; set; }

    public virtual SettingCategory? SettingCategory { get; set; }

    public virtual SettingsStatus? SettingStatus { get; set; }
}
