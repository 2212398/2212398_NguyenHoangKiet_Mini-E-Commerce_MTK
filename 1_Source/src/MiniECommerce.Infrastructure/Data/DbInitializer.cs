using MiniECommerce.Core.Entities;
using System.Text.Json;

namespace MiniECommerce.Infrastructure.Data;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();
        
        // Check if already seeded
        if (context.Users.Any())
        {
            return;
        }
        
        // Seed Users
        var users = new[]
        {
            new User
            {
                Email = "admin@ecommerce.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "Admin"
            },
            new User
            {
                Email = "user@ecommerce.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                Role = "User"
            }
        };
        context.Users.AddRange(users);
        context.SaveChanges();
        
        // Seed Products
        var products = new[]
        {
            new Product { Name = "Laptop Dell XPS 13", Category = "Electronics", Price = 25000000, Weight = 1.2, Stock = 50, Description = "High-performance laptop" },
            new Product { Name = "iPhone 15 Pro", Category = "Electronics", Price = 30000000, Weight = 0.2, Stock = 100, Description = "Latest iPhone model" },
            new Product { Name = "Samsung 55\" Smart TV", Category = "Electronics", Price = 15000000, Weight = 18.5, Stock = 30, Description = "4K Smart TV" },
            new Product { Name = "Nike Air Max", Category = "Fashion", Price = 3500000, Weight = 0.8, Stock = 80, Description = "Running shoes" },
            new Product { Name = "The Art of War Book", Category = "Books", Price = 150000, Weight = 0.3, Stock = 200, Description = "Classic strategy book" },
            new Product { Name = "Coffee Maker", Category = "Home", Price = 2500000, Weight = 3.5, Stock = 40, Description = "Automatic coffee maker" },
            new Product { Name = "Gaming Mouse", Category = "Electronics", Price = 800000, Weight = 0.15, Stock = 150, Description = "RGB gaming mouse" },
            new Product { Name = "Office Chair", Category = "Furniture", Price = 4500000, Weight = 15.0, Stock = 25, Description = "Ergonomic office chair" },
        };
        context.Products.AddRange(products);
        context.SaveChanges();
        
        // Seed Shipping Methods
        var standardParams = new
        {
            BaseFee = 20000m,
            PerKgFee = 5000m,
            RegionFactors = new Dictionary<string, decimal>
            {
                { "North", 1.0m },
                { "Central", 1.2m },
                { "South", 1.5m }
            }
        };
        
        var expressParams = new
        {
            BaseFee = 30000m,
            BaseMultiplier = 1.2m,
            PerKgFee = 8000m,
            PeakHourSurge = 15000m
        };
        
        var sameDayParams = new
        {
            BaseFee = 50000m,
            PerKmFee = 3000m,
            CutoffHour = 14
        };
        
        var ecoParams = new
        {
            BaseFee = 15000m,
            PerKgFee = 3000m,
            BulkWeightThreshold = 10.0,
            BulkDiscount = 0.15m
        };
        
        var shippingMethods = new[]
        {
            new ShippingMethod
            {
                Code = "STANDARD",
                DisplayName = "Standard Shipping (3-5 days)",
                ParamsJSON = JsonSerializer.Serialize(standardParams)
            },
            new ShippingMethod
            {
                Code = "EXPRESS",
                DisplayName = "Express Shipping (1-2 days)",
                ParamsJSON = JsonSerializer.Serialize(expressParams)
            },
            new ShippingMethod
            {
                Code = "SAME_DAY",
                DisplayName = "Same-Day Shipping",
                ParamsJSON = JsonSerializer.Serialize(sameDayParams)
            },
            new ShippingMethod
            {
                Code = "ECO",
                DisplayName = "Eco Shipping (5-7 days)",
                ParamsJSON = JsonSerializer.Serialize(ecoParams)
            }
        };
        context.ShippingMethods.AddRange(shippingMethods);
        context.SaveChanges();
    }
}
