using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineShoppingSystem_Main.Models;

[Table("CartItem")]
public partial class CartItem
{
    [Key]
    [StringLength(255)]
    [Unicode(false)]
    public string CartItemId { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? CartId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? ProductId { get; set; }

    public int Quantity { get; set; }

    public double PriceEachItem { get; set; }

    [ForeignKey("CartId")]
    [InverseProperty("CartItems")]
    public virtual Cart? Cart { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("CartItems")]
    public virtual Product? Product { get; set; }
}
