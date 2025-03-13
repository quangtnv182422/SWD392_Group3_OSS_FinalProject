using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Data.Models;

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
    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
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
			entity.HasKey(e => e.CartId).HasName("PK__Cart__51BCD7B7E0E01DD5");

			entity.ToTable("Cart");

			entity.Property(e => e.CustomerId).HasMaxLength(450);

			entity.HasOne(d => d.Customer).WithMany(p => p.Carts)
				.HasForeignKey(d => d.CustomerId)
				.HasConstraintName("FK__Cart__CustomerId__75A278F5");
		});

		modelBuilder.Entity<CartItem>(entity =>
		{
			entity.HasKey(e => e.CartItemId).HasName("PK__CartItem__488B0B0AC955D3DD");

			entity.ToTable("CartItem");

			entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
				.HasForeignKey(d => d.CartId)
				.HasConstraintName("FK__CartItem__CartId__76969D2E");

			entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
				.HasForeignKey(d => d.ProductId)
				.HasConstraintName("FK__CartItem__Produc__778AC167");
		});

		modelBuilder.Entity<Category>(entity =>
		{
			entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A0B95A48F83");

			entity.ToTable("Category");

			entity.Property(e => e.CategoryName).HasMaxLength(255);
		});

		modelBuilder.Entity<Order>(entity =>
		{
			entity.HasKey(e => e.OrderId).HasName("PK__Order__C3905BCF54F2F264");

			entity.ToTable("Order");

			entity.Property(e => e.Address)
				.HasMaxLength(255)
				.IsUnicode(true);
			entity.Property(e => e.CustomerId).HasMaxLength(450);
			entity.Property(e => e.Email).HasMaxLength(255);
			entity.Property(e => e.FullName).HasMaxLength(255);
			entity.Property(e => e.Note)
				.HasMaxLength(255)
				.IsUnicode(true);
			entity.Property(e => e.OrderedAt).HasColumnType("datetime");
			entity.Property(e => e.PaymentMethod).HasMaxLength(255);
			entity.Property(e => e.PhoneNumber).HasMaxLength(20);
			entity.Property(e => e.StaffId).HasMaxLength(450);

			entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
				.HasForeignKey(d => d.CustomerId)
				.HasConstraintName("FK__Order__CustomerI__787EE5A0");

			entity.HasOne(d => d.OrderStatus).WithMany(p => p.Orders)
				.HasForeignKey(d => d.OrderStatusId)
				.HasConstraintName("FK__Order__OrderStat__797309D9");
		});

		modelBuilder.Entity<OrderItem>(entity =>
		{
			entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED06812D9F44D4");

			entity.ToTable("OrderItem");

			entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
				.HasForeignKey(d => d.OrderId)
				.HasConstraintName("FK__OrderItem__Order__7A672E12");

			entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
				.HasForeignKey(d => d.ProductId)
				.HasConstraintName("FK__OrderItem__Produ__7B5B524B");
		});

		modelBuilder.Entity<OrderStatus>(entity =>
		{
			entity.HasKey(e => e.StatusId).HasName("PK__OrderSta__C8EE20634002C90E");

			entity.ToTable("OrderStatus");

			entity.Property(e => e.StatusName).HasMaxLength(255);
		});

		modelBuilder.Entity<Product>(entity =>
		{
			entity.HasKey(e => e.ProductId).HasName("PK__Product__B40CC6CD6D3638C6");

			entity.ToTable("Product");

			entity.Property(e => e.CreatedAt).HasColumnType("datetime");
			entity.Property(e => e.Description).HasMaxLength(255);
			entity.Property(e => e.IsFeatured).HasDefaultValue(true);
			entity.Property(e => e.ProductName).HasMaxLength(255);

			entity.HasOne(d => d.Category).WithMany(p => p.Products)
				.HasForeignKey(d => d.CategoryId)
				.HasConstraintName("FK__Product__Categor__7C4F7684");

			entity.HasOne(d => d.ProductStatus).WithMany(p => p.Products)
				.HasForeignKey(d => d.ProductStatusId)
				.HasConstraintName("FK__Product__Product__7D439ABD");
		});

		modelBuilder.Entity<ProductImage>(entity =>
		{
			entity.HasKey(e => e.ProductImageId).HasName("PK__ProductI__07B2B1B8C139F9CC");

			entity.ToTable("ProductImage");

			entity.Property(e => e.ProductImageUrl)
				.HasMaxLength(255)
				.IsUnicode(false)
				.HasColumnName("ProductImageURL");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages).HasConstraintName("FK__ProductIm__Produ__7C4F7684");
        });

		modelBuilder.Entity<ProductStatus>(entity =>
		{
			entity.HasKey(e => e.ProductStatusId).HasName("PK__ProductS__2082058BB6EC726F");

			entity.ToTable("ProductStatus");

			entity.Property(e => e.StatusDescription)
				.HasMaxLength(255)
				.IsUnicode(false);
		});

		modelBuilder.Entity<Setting>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PK__Settings__3214EC07A1ADA655");

			entity.Property(e => e.SettingName).HasMaxLength(255);
			entity.Property(e => e.SettingValue).HasMaxLength(255);

			entity.HasOne(d => d.SettingCategory).WithMany(p => p.Settings)
				.HasForeignKey(d => d.SettingCategoryId)
				.HasConstraintName("FK__Settings__Settin__7F2BE32F");

			entity.HasOne(d => d.SettingStatus).WithMany(p => p.Settings)
				.HasForeignKey(d => d.SettingStatusId)
				.HasConstraintName("FK__Settings__Settin__00200768");
		});

		modelBuilder.Entity<SettingCategory>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PK__SettingC__3214EC07E1169F2E");

			entity.ToTable("SettingCategory");

			entity.Property(e => e.CategoryName).HasMaxLength(255);
		});

		modelBuilder.Entity<SettingsStatus>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PK__Settings__3214EC07EB1BB1FE");

			entity.ToTable("SettingsStatus");

			entity.Property(e => e.StatusName).HasMaxLength(255);
		});

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
