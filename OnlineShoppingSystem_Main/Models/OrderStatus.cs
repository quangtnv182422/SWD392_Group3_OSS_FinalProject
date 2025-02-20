using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineShoppingSystem_Main.Models;

[Table("OrderStatus")]
public partial class OrderStatus
{
    [Key]
    public int StatusId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string StatusName { get; set; } = null!;

    [InverseProperty("OrderStatus")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
