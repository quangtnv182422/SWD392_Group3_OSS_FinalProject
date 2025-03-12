using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public DateTime OrderedAt { get; set; }

    public string? PaymentMethod { get; set; }

    public string? Note { get; set; }

    public string? Address { get; set; }

    public string? CustomerId { get; set; }

    public string? StaffId { get; set; }

    public int? OrderStatusId { get; set; }

    public float TotalCost { get; set; }

    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public virtual AspNetUser? Customer { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual OrderStatus? OrderStatus { get; set; }
}
