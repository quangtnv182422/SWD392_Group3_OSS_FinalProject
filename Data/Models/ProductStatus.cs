using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Data.Models;

public partial class ProductStatus
{
    public int ProductStatusId { get; set; }

    public string StatusDescription { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
