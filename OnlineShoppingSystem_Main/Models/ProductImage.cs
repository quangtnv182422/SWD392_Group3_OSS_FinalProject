using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineShoppingSystem_Main.Models;

[Table("ProductImage")]
public partial class ProductImage
{
    [Key]
    public int ProductImageId { get; set; }

    public int? ProductId { get; set; }

    [Column("ProductImageURL")]
    [StringLength(255)]
    [Unicode(false)]
    public string ProductImageUrl { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("ProductImages")]
    public virtual Product? Product { get; set; }
}
