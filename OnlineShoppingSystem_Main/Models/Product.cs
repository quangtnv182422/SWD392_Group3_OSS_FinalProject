using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineShoppingSystem_Main.Models;

[Table("Product")]
public partial class Product
{
    [Key]
    [StringLength(255)]
    [Unicode(false)]
    public string ProductId { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string ProductName { get; set; } = null!;

    public int Quantity { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Description { get; set; }

    public double Price { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? CategoryId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? ProductStatusId { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category? Category { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [InverseProperty("Product")]
    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    [ForeignKey("ProductStatusId")]
    [InverseProperty("Products")]
    public virtual ProductStatus? ProductStatus { get; set; }
}
