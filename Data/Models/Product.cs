using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Models;

public partial class Product
{
    public int ProductId { get; set; }
    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]

    public string ProductName { get; set; } = null!;

    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]

    public int Quantity { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]

    public string? Description { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]

    public double Price { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Sale price must be a positive value.")]

    public double? SalePrice { get; set; }


    public DateTime CreatedAt { get; set; }

    [Required(ErrorMessage = "Category is required.")]

    public int? CategoryId { get; set; }

    [Required(ErrorMessage = "Product status is required.")]

    public int? ProductStatusId { get; set; }

    public bool IsFeatured { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ProductStatus? ProductStatus { get; set; }
}
