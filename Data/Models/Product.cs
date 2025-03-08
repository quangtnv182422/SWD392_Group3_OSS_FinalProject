using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public int Quantity { get; set; }

    public string? Description { get; set; }

    public double Price { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CategoryId { get; set; }

    public int? ProductStatusId { get; set; }

    public bool IsFeatured { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ProductStatus? ProductStatus { get; set; }
}
