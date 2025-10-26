using MiniECommerce.Core.Interfaces;
using MiniECommerce.Core.Models;
using MiniECommerce.Core.Strategies;
using Xunit;

namespace MiniECommerce.Tests.Strategies;

/// <summary>
/// Unit tests for Strategy Pattern implementations
/// Testing different shipping calculation strategies
/// </summary>
public class ShippingStrategyTests
{
    [Fact]
    public void StandardStrategy_CalculatesCorrectFee_WithDefaultParams()
    {
        // Arrange
        var strategy = new StandardShippingStrategy();
        var context = new OrderContext
        {
            Weight = 2.0,
            Distance = 15.0,
            Region = "North",
            Subtotal = 1000000
        };
        
        // Act
        var fee = strategy.Calculate(context);
        
        // Assert
        // Base(20000) + PerKg(5000) * Weight(2) * RegionFactor[North](1.0) = 30000
        Assert.Equal(30000, fee);
    }
    
    [Fact]
    public void StandardStrategy_AppliesRegionFactor_ForSouth()
    {
        // Arrange
        var strategy = new StandardShippingStrategy();
        var context = new OrderContext
        {
            Weight = 2.0,
            Region = "South",
            Subtotal = 1000000
        };
        
        // Act
        var fee = strategy.Calculate(context);
        
        // Assert
        // (Base(20000) + PerKg(5000) * Weight(2)) * RegionFactor[South](1.5) = 45000
        Assert.Equal(45000, fee);
    }
    
    [Theory]
    [InlineData(0, 20000)]     // Zero weight
    [InlineData(5.5, 47500)]   // 5.5kg
    [InlineData(10, 70000)]    // 10kg
    public void StandardStrategy_VariousWeights_CalculatesCorrectly(double weight, decimal expectedFee)
    {
        // Arrange
        var strategy = new StandardShippingStrategy();
        var context = new OrderContext
        {
            Weight = weight,
            Region = "North",
            Subtotal = 1000000
        };
        
        // Act
        var fee = strategy.Calculate(context);
        
        // Assert
        Assert.Equal(expectedFee, fee);
    }
    
    [Fact]
    public void ExpressStrategy_CalculatesCorrectFee_NonPeakHour()
    {
        // Arrange
        var strategy = new ExpressShippingStrategy();
        var context = new OrderContext
        {
            Weight = 2.0,
            OrderTime = new DateTime(2024, 1, 1, 10, 0, 0), // 10 AM - non-peak
            Subtotal = 1000000
        };
        
        // Act
        var fee = strategy.Calculate(context);
        
        // Assert
        // Base(30000) * Multiplier(1.2) + PerKg(8000) * Weight(2) = 52000
        Assert.Equal(52000, fee);
    }
    
    [Fact]
    public void ExpressStrategy_AddsSurge_DuringPeakHours()
    {
        // Arrange
        var strategy = new ExpressShippingStrategy();
        var context = new OrderContext
        {
            Weight = 2.0,
            OrderTime = new DateTime(2024, 1, 1, 8, 0, 0), // 8 AM - peak hour
            Subtotal = 1000000
        };
        
        // Act
        var fee = strategy.Calculate(context);
        
        // Assert
        // Base(30000) * Multiplier(1.2) + PerKg(8000) * Weight(2) + PeakSurge(15000) = 67000
        Assert.Equal(67000, fee);
    }
    
    [Theory]
    [InlineData(7, 67000)]   // 7 AM - peak
    [InlineData(8, 67000)]   // 8 AM - peak
    [InlineData(10, 52000)]  // 10 AM - non-peak
    [InlineData(17, 67000)]  // 5 PM - peak
    [InlineData(18, 67000)]  // 6 PM - peak
    [InlineData(19, 52000)]  // 7 PM - non-peak
    public void ExpressStrategy_PeakHourBoundaries_CalculatesCorrectly(int hour, decimal expectedFee)
    {
        // Arrange
        var strategy = new ExpressShippingStrategy();
        var context = new OrderContext
        {
            Weight = 2.0,
            OrderTime = new DateTime(2024, 1, 1, hour, 0, 0),
            Subtotal = 1000000
        };
        
        // Act
        var fee = strategy.Calculate(context);
        
        // Assert
        Assert.Equal(expectedFee, fee);
    }
    
    [Fact]
    public void SameDayStrategy_CalculatesCorrectFee_BeforeCutoff()
    {
        // Arrange
        var strategy = new SameDayShippingStrategy();
        var context = new OrderContext
        {
            Distance = 10.0,
            OrderTime = new DateTime(2024, 1, 1, 13, 0, 0), // 1 PM - before 2 PM cutoff
            Weight = 2.0,
            Subtotal = 1000000
        };
        
        // Act
        var fee = strategy.Calculate(context);
        
        // Assert
        // Base(50000) + PerKm(3000) * Distance(10) = 80000
        Assert.Equal(80000, fee);
    }
    
    [Fact]
    public void SameDayStrategy_ThrowsException_AfterCutoff()
    {
        // Arrange
        var strategy = new SameDayShippingStrategy();
        var context = new OrderContext
        {
            Distance = 10.0,
            OrderTime = new DateTime(2024, 1, 1, 14, 30, 0), // 2:30 PM - after cutoff
            Weight = 2.0,
            Subtotal = 1000000
        };
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => strategy.Calculate(context));
    }
    
    [Theory]
    [InlineData(0, 50000)]    // Zero distance
    [InlineData(5, 65000)]    // 5 km
    [InlineData(20, 110000)]  // 20 km
    public void SameDayStrategy_VariousDistances_CalculatesCorrectly(double distance, decimal expectedFee)
    {
        // Arrange
        var strategy = new SameDayShippingStrategy();
        var context = new OrderContext
        {
            Distance = distance,
            OrderTime = new DateTime(2024, 1, 1, 10, 0, 0),
            Weight = 2.0,
            Subtotal = 1000000
        };
        
        // Act
        var fee = strategy.Calculate(context);
        
        // Assert
        Assert.Equal(expectedFee, fee);
    }
    
    [Fact]
    public void EcoStrategy_CalculatesCorrectFee_BelowBulkThreshold()
    {
        // Arrange
        var strategy = new EcoShippingStrategy();
        var context = new OrderContext
        {
            Weight = 5.0, // Below 10kg threshold
            Subtotal = 1000000
        };
        
        // Act
        var fee = strategy.Calculate(context);
        
        // Assert
        // Base(15000) + PerKg(3000) * Weight(5) = 30000
        Assert.Equal(30000, fee);
    }
    
    [Fact]
    public void EcoStrategy_AppliesBulkDiscount_AboveThreshold()
    {
        // Arrange
        var strategy = new EcoShippingStrategy();
        var context = new OrderContext
        {
            Weight = 12.0, // Above 10kg threshold
            Subtotal = 1000000
        };
        
        // Act
        var fee = strategy.Calculate(context);
        
        // Assert
        // (Base(15000) + PerKg(3000) * Weight(12)) * (1 - Discount(0.15)) = 43350
        Assert.Equal(43350, fee);
    }
    
    [Theory]
    [InlineData(9.9, 44700)]   // Just below threshold - no discount
    [InlineData(10.0, 38250)]  // Exactly at threshold - with discount
    [InlineData(10.1, 38505)]  // Just above threshold - with discount
    public void EcoStrategy_BulkThresholdBoundary_CalculatesCorrectly(double weight, decimal expectedFee)
    {
        // Arrange
        var strategy = new EcoShippingStrategy();
        var context = new OrderContext
        {
            Weight = weight,
            Subtotal = 1000000
        };
        
        // Act
        var fee = strategy.Calculate(context);
        
        // Assert
        Assert.Equal(expectedFee, fee);
    }
    
    [Fact]
    public void AllStrategies_SameContext_ProduceDifferentFees()
    {
        // Arrange
        var context = new OrderContext
        {
            Weight = 5.0,
            Distance = 15.0,
            Region = "North",
            Subtotal = 1000000,
            OrderTime = new DateTime(2024, 1, 1, 10, 0, 0)
        };
        
        var standardStrategy = new StandardShippingStrategy();
        var expressStrategy = new ExpressShippingStrategy();
        var sameDayStrategy = new SameDayShippingStrategy();
        var ecoStrategy = new EcoShippingStrategy();
        
        // Act
        var standardFee = standardStrategy.Calculate(context);
        var expressFee = expressStrategy.Calculate(context);
        var sameDayFee = sameDayStrategy.Calculate(context);
        var ecoFee = ecoStrategy.Calculate(context);
        
        // Assert - All fees should be different
        Assert.NotEqual(standardFee, expressFee);
        Assert.NotEqual(standardFee, sameDayFee);
        Assert.NotEqual(standardFee, ecoFee);
        Assert.NotEqual(expressFee, sameDayFee);
    }
    
    [Fact]
    public void StandardStrategy_CustomParams_AppliesCorrectly()
    {
        // Arrange
        var customParams = @"{
            ""BaseFee"": 30000,
            ""PerKgFee"": 7000,
            ""RegionFactors"": {
                ""North"": 1.0,
                ""South"": 2.0
            }
        }";
        
        var strategy = new StandardShippingStrategy(customParams);
        var context = new OrderContext
        {
            Weight = 3.0,
            Region = "South",
            Subtotal = 1000000
        };
        
        // Act
        var fee = strategy.Calculate(context);
        
        // Assert
        // (Base(30000) + PerKg(7000) * Weight(3)) * RegionFactor[South](2.0) = 102000
        Assert.Equal(102000, fee);
    }
    
    [Fact]
    public void AllStrategies_ProvideCalculationDetails()
    {
        // Arrange
        var context = new OrderContext
        {
            Weight = 2.0,
            Distance = 10.0,
            Region = "North",
            Subtotal = 1000000,
            OrderTime = new DateTime(2024, 1, 1, 10, 0, 0)
        };
        
        var strategies = new (IShippingStrategy Strategy, string Name)[]
        {
            (new StandardShippingStrategy(), "Standard"),
            (new ExpressShippingStrategy(), "Express"),
            (new SameDayShippingStrategy(), "Same-Day"),
            (new EcoShippingStrategy(), "Eco")
        };
        
        // Act & Assert
        foreach (var item in strategies)
        {
            var details = item.Strategy.GetCalculationDetails(context);
            Assert.NotNull(details);
            Assert.NotEmpty(details);
            Assert.Contains(item.Name, details, StringComparison.OrdinalIgnoreCase);
        }
    }
}
