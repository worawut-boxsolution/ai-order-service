using Microsoft.EntityFrameworkCore;
using OrderApi.Data;
using OrderApi.Models;
using OrderApi.Services;
using Xunit;

namespace OrderApi.Tests;

public class OrderServiceTests
{
    private OrderDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new OrderDbContext(options);
    }

    [Fact]
    public async Task CreateOrder_WithSAVE10_ShouldApply10PercentDiscount()
    {
        // Arrange
        using var context = GetDbContext();
        var service = new OrderService(context);
        var request = new CreateOrderRequest
        {
            CustomerName = "Test Customer",
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductName = "Product 1", Price = 1000m, Quantity = 1 }
            },
            CouponCode = "SAVE10"
        };

        // Act
        var result = await service.CreateOrderAsync(request);

        // Assert
        Assert.Equal(1000m, result.TotalAmount);
        Assert.Equal(100m, result.DiscountAmount); // 10% of 1000
    }

    [Fact]
    public async Task CreateOrder_WithFLAT50_ShouldApply50Discount()
    {
        // Arrange
        using var context = GetDbContext();
        var service = new OrderService(context);
        var request = new CreateOrderRequest
        {
            CustomerName = "Test Customer",
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductName = "Product 1", Price = 1000m, Quantity = 1 }
            },
            CouponCode = "FLAT50"
        };

        // Act
        var result = await service.CreateOrderAsync(request);

        // Assert
        Assert.Equal(1000m, result.TotalAmount);
        Assert.Equal(50m, result.DiscountAmount);
    }

    [Fact]
    public async Task CreateOrder_WithoutCoupon_ShouldHaveZeroDiscount()
    {
        // Arrange
        using var context = GetDbContext();
        var service = new OrderService(context);
        var request = new CreateOrderRequest
        {
            CustomerName = "Test Customer",
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductName = "Product 1", Price = 1000m, Quantity = 1 }
            },
            CouponCode = ""
        };

        // Act
        var result = await service.CreateOrderAsync(request);

        // Assert
        Assert.Equal(0m, result.DiscountAmount);
    }

    [Fact]
    public async Task CreateOrder_TaxCalculation_ShouldBe7PercentExclude()
    {
        // Arrange
        // Scenario: Total 1000, SAVE10 -> Discount 100. Remaining 900.
        // Tax = 900 * 0.07 = 63.
        // Net = 900 + 63 = 963.
        using var context = GetDbContext();
        var service = new OrderService(context);
        var request = new CreateOrderRequest
        {
            CustomerName = "Test Customer",
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductName = "Product 1", Price = 1000m, Quantity = 1 }
            },
            CouponCode = "SAVE10"
        };

        // Act
        var result = await service.CreateOrderAsync(request);

        // Assert
        Assert.Equal(100m, result.DiscountAmount);
        Assert.Equal(63m, result.TaxAmount);
        Assert.Equal(963m, result.NetAmount);
    }

    [Fact]
    public async Task CreateOrder_NegativeAmount_ShouldThrowArgumentException()
    {
        // Arrange
        using var context = GetDbContext();
        var service = new OrderService(context);
        var request = new CreateOrderRequest
        {
            CustomerName = "Test Customer",
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductName = "Bad Product", Price = -100m, Quantity = 1 }
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateOrderAsync(request));
    }
}
