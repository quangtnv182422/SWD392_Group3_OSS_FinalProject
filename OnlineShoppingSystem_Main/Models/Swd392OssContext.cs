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

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

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

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Cart__51BCD7B76047707B");

            entity.HasOne(d => d.Customer).WithMany(p => p.Carts).HasConstraintName("FK__Cart__CustomerId__7C4F7684");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("PK__CartItem__488B0B0AFAB7CF07");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems).HasConstraintName("FK__CartItem__CartId__10566F31");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems).HasConstraintName("FK__CartItem__Produc__114A936A");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A0B60D57E87");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Order__C3905BCF68B202ED");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders).HasConstraintName("FK__Order__CustomerI__208CD6FA");

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.Orders).HasConstraintName("FK__Order__OrderStat__2180FB33");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED0681430935F5");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems).HasConstraintName("FK__OrderItem__Order__245D67DE");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems).HasConstraintName("FK__OrderItem__Produ__25518C17");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__OrderSta__C8EE20637130017D");

            entity.Property(e => e.StatusId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__B40CC6CDA089A8A1");

            entity.HasOne(d => d.Category).WithMany(p => p.Products).HasConstraintName("FK__Product__Categor__09A971A2");

            entity.HasOne(d => d.ProductStatus).WithMany(p => p.Products).HasConstraintName("FK__Product__Product__0A9D95DB");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.ProductImageId).HasName("PK__ProductI__07B2B1B881C97387");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages).HasConstraintName("FK__ProductIm__Produ__0D7A0286");
        });

        modelBuilder.Entity<ProductStatus>(entity =>
        {
            entity.HasKey(e => e.ProductStatusId).HasName("PK__ProductS__2082058B62435A97");
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Settings__3214EC07D2BA0E09");

            entity.HasOne(d => d.SettingCategory).WithMany(p => p.Settings).HasConstraintName("FK__Settings__Settin__75A278F5");

            entity.HasOne(d => d.SettingStatus).WithMany(p => p.Settings).HasConstraintName("FK__Settings__Settin__76969D2E");
        });

        modelBuilder.Entity<SettingCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SettingC__3214EC07D3E2FCA6");
        });

        modelBuilder.Entity<SettingsStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Settings__3214EC070DE69AD2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
