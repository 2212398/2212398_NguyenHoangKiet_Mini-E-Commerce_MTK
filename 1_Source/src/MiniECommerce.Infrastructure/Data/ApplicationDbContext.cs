using Microsoft.EntityFrameworkCore;
using MiniECommerce.Core.Entities;

namespace MiniECommerce.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ShippingMethod> ShippingMethods { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).HasMaxLength(50);
        });
        
        // Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
        });
        
        // ShippingMethod
        modelBuilder.Entity<ShippingMethod>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(255);
        });
        
        // Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Subtotal).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Discount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Tax).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ShippingFee).HasColumnType("decimal(18,2)");
            entity.Property(e => e.GrandTotal).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Region).HasMaxLength(50);
            
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);
        });
        
        // OrderItem
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.LineTotal).HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(e => e.OrderId);
                
            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId);
        });
        
        // CartItem
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.ProductId }).IsUnique();
            
            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId);
        });
    }
}
