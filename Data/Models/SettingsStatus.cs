using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class SettingsStatus
{
    public int Id { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Setting> Settings { get; set; } = new List<Setting>();
}
