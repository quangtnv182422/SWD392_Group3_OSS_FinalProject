﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Models;

public partial class AspNetRoleClaim
{
    [Key]
    public int Id { get; set; }

    [StringLength(450)]
    public string RoleId { get; set; } = null!;

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("AspNetRoleClaims")]
    public virtual AspNetRole Role { get; set; } = null!;
}