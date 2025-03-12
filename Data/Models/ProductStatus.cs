using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class ProductStatus
{
    public int ProductStatusId { get; set; }

    public string StatusDescription { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
