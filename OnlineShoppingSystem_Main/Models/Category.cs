using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OnlineShoppingSystem_Main.Models;

[Table("Category")]
public partial class Category
{
    [Key]
    [StringLength(255)]
    [Unicode(false)]
    public string CategoryId { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string CategoryName { get; set; } = null!;

    [InverseProperty("Category")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
