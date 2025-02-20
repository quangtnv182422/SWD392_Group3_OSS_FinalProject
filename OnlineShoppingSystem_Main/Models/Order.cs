using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineShoppingSystem_Main.Models;

[Table("Order")]
public partial class Order
{
    [Key]
    [StringLength(255)]
    [Unicode(false)]
    public string OrderId { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime OrderedAt { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? PaymentMethod { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Note { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Address { get; set; }

    [StringLength(450)]
    public string? CustomerId { get; set; }

    [StringLength(450)]
    public string? StaffId { get; set; }

    public int? OrderStatusId { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Orders")]
    public virtual AspNetUser? Customer { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [ForeignKey("OrderStatusId")]
    [InverseProperty("Orders")]
    public virtual OrderStatus? OrderStatus { get; set; }
}
