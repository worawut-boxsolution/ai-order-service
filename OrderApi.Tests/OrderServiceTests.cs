using Microsoft.EntityFrameworkCore;
using OrderApi.Data;
using OrderApi.Models;
using OrderApi.Services;
using Xunit;

namespace OrderApi.Tests;

public class OrderServiceTests
{
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _service = new OrderService();
    }

    [Fact]
    public void CalculateNetAmount_WithDiscount_ReturnsExpectedValue()
    {
        // Arrange
        decimal total = 1000;
        decimal discount = 100;
        // Calculation: (1000 - 100) = 900; 900 + (900 * 0.07) = 963
        decimal expected = 963.00m;

        // Act
        var result = _service.CalculateNetAmount(total, discount);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateNetAmount_WithoutDiscount_ReturnsExpectedValue()
    {
        // Arrange
        decimal total = 500;
        decimal discount = 0;
        // Calculation: (500 - 0) = 500; 500 + (500 * 0.07) = 535
        decimal expected = 535.00m;

        // Act
        var result = _service.CalculateNetAmount(total, discount);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task AddOrderToDatabase_PersistsCorrectData()
    {
        // Arrange: ใช้ Unique database name เพื่อไม่ให้ชนกับ test อื่น
        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseInMemoryDatabase(databaseName: "OrderTestDb_" + Guid.NewGuid().ToString())
            .Options;

        using (var context = new OrderDbContext(options))
        {
            var order = new Order 
            { 
                CustomerName = "Test Customer", 
                TotalAmount = 1000, 
                Discount = 100, 
                NetAmount = _service.CalculateNetAmount(1000, 100), // 963
                CreatedAt = DateTime.UtcNow
            };

            // Act
            context.Orders.Add(order);
            await context.SaveChangesAsync();
        }

        // Assert: ตรวจสอบข้อมูลใน Database
        using (var context = new OrderDbContext(options))
        {
            var count = await context.Orders.CountAsync();
            var savedOrder = await context.Orders.FirstOrDefaultAsync();
            
            Assert.Equal(1, count);
            Assert.NotNull(savedOrder);
            Assert.Equal("Test Customer", savedOrder.CustomerName);
            Assert.Equal(963.00m, savedOrder.NetAmount);
        }
    }
}
