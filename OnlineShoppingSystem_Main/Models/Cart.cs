using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineShoppingSystem_Main.Models;

[Table("Cart")]
public partial class Cart
{
    [Key]
    [StringLength(255)]
    [Unicode(false)]
    public string CartId { get; set; } = null!;

    public double TotalPrice { get; set; }

    [StringLength(450)]
    public string? CustomerId { get; set; }

    [InverseProperty("Cart")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [ForeignKey("CustomerId")]
    [InverseProperty("Carts")]
    public virtual AspNetUser? Customer { get; set; }
}
