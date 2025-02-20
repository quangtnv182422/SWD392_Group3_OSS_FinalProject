using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineShoppingSystem_Main.Models;

[Table("ProductStatus")]
public partial class ProductStatus
{
    [Key]
    [StringLength(255)]
    [Unicode(false)]
    public string ProductStatusId { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string StatusDescription { get; set; } = null!;

    [InverseProperty("ProductStatus")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
