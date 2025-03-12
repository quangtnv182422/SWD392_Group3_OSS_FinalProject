using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class SettingCategory
{
    public int Id { get; set; }

    public string CategoryName { get; set; } = null!;

    public virtual ICollection<Setting> Settings { get; set; } = new List<Setting>();
}
