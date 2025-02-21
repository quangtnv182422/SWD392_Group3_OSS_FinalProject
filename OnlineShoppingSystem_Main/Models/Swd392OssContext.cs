using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OnlineShoppingSystem_Main.Models;

public partial class Swd392OssContext : IdentityDbContext
{
    public Swd392OssContext()
    {
    }

    public Swd392OssContext(DbContextOptions<Swd392OssContext> options)
        : base(options)
    {
    }

   
    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<ProductStatus> ProductStatuses { get; set; }

    public virtual DbSet<Setting> Settings { get; set; }

    public virtual DbSet<SettingCategory> SettingCategories { get; set; }

    public virtual DbSet<SettingsStatus> SettingsStatuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var ConnectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(ConnectionString);
        }

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Cart__51BCD7B783078B66");

            entity.Property(e => e.CartId).ValueGeneratedNever();

            entity.HasOne(d => d.Customer).WithMany(p => p.Carts).HasConstraintName("FK__Cart__CustomerId__73BA3083");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("PK__CartItem__488B0B0AD89A9D68");

            entity.Property(e => e.CartItemId).ValueGeneratedNever();

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems).HasConstraintName("FK__CartItem__CartId__74AE54BC");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems).HasConstraintName("FK__CartItem__Produc__75A278F5");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A0B16135AAE");

            entity.Property(e => e.CategoryId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Order__C3905BCF506E0C4F");

            entity.Property(e => e.OrderId).ValueGeneratedNever();

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders).HasConstraintName("FK__Order__CustomerI__76969D2E");

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.Orders).HasConstraintName("FK__Order__OrderStat__778AC167");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED06819EFA39A1");

            entity.Property(e => e.OrderItemId).ValueGeneratedNever();

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems).HasConstraintName("FK__OrderItem__Order__787EE5A0");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems).HasConstraintName("FK__OrderItem__Produ__797309D9");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__OrderSta__C8EE206368290E58");

            entity.Property(e => e.StatusId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__B40CC6CDB565A516");

            entity.Property(e => e.ProductId).ValueGeneratedNever();

            entity.HasOne(d => d.Category).WithMany(p => p.Products).HasConstraintName("FK__Product__Categor__7A672E12");

            entity.HasOne(d => d.ProductStatus).WithMany(p => p.Products).HasConstraintName("FK__Product__Product__7B5B524B");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.ProductImageId).HasName("PK__ProductI__07B2B1B8736038EC");

            entity.Property(e => e.ProductImageId).ValueGeneratedNever();

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages).HasConstraintName("FK__ProductIm__Produ__7C4F7684");
        });

        modelBuilder.Entity<ProductStatus>(entity =>
        {
            entity.HasKey(e => e.ProductStatusId).HasName("PK__ProductS__2082058B41599682");

            entity.Property(e => e.ProductStatusId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Settings__3214EC075FF73A0E");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.SettingCategory).WithMany(p => p.Settings).HasConstraintName("FK__Settings__Settin__7D439ABD");

            entity.HasOne(d => d.SettingStatus).WithMany(p => p.Settings).HasConstraintName("FK__Settings__Settin__7E37BEF6");
        });

        modelBuilder.Entity<SettingCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SettingC__3214EC072622F75D");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<SettingsStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Settings__3214EC07AC166F9E");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
