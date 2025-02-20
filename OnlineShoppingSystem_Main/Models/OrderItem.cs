﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineShoppingSystem_Main.Models;

[Table("OrderItem")]
public partial class OrderItem
{
    [Key]
    [StringLength(255)]
    [Unicode(false)]
    public string OrderItemId { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? OrderId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? ProductId { get; set; }

    public int Quantity { get; set; }

    public double PriceEachItem { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderItems")]
    public virtual Order? Order { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("OrderItems")]
    public virtual Product? Product { get; set; }
}
