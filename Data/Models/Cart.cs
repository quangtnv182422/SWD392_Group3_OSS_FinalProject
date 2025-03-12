using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public double TotalPrice { get; set; }

    public string? CustomerId { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual AspNetUser? Customer { get; set; }
}
