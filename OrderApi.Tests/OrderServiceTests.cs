using OrderApi.DTOs;
using OrderApi.Services;
using Xunit;

namespace OrderApi.Tests
{
    public class OrderServiceTests
    {
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _orderService = new OrderService();
        }

        [Fact]
        public void CalculateOrder_WithSAVE10_ShouldDiscount10Percent()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                CustomerName = "John Doe",
                Items = new List<OrderItemRequest>
                {
                    new OrderItemRequest { ProductName = "Item 1", Price = 1000m, Quantity = 1 }
                },
                CouponCode = "SAVE10"
            };

            // Act
            var result = _orderService.CalculateOrder(request);

            // Assert
            // Total = 1000
            // Discount = 1000 * 0.10 = 100
            // Taxable = 900
            // Tax = 900 * 0.07 = 63
            // Net = 963
            Assert.Equal(1000m, result.TotalAmount);
            Assert.Equal(100m, result.DiscountAmount);
            Assert.Equal(63m, result.TaxAmount);
            Assert.Equal(963m, result.NetAmount);
        }

        [Fact]
        public void CalculateOrder_WithFLAT50_ShouldDiscount50Baht()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                CustomerName = "John Doe",
                Items = new List<OrderItemRequest>
                {
                    new OrderItemRequest { ProductName = "Item 1", Price = 1000m, Quantity = 1 }
                },
                CouponCode = "FLAT50"
            };

            // Act
            var result = _orderService.CalculateOrder(request);

            // Assert
            // Total = 1000
            // Discount = 50
            // Taxable = 950
            // Tax = 950 * 0.07 = 66.5
            // Net = 1016.5
            Assert.Equal(1000m, result.TotalAmount);
            Assert.Equal(50m, result.DiscountAmount);
            Assert.Equal(66.50m, result.TaxAmount);
            Assert.Equal(1016.50m, result.NetAmount);
        }

        [Fact]
        public void CalculateOrder_WithFLAT50_AndTotalLessThan50_ShouldDiscountTotalAmount()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                CustomerName = "John Doe",
                Items = new List<OrderItemRequest>
                {
                    new OrderItemRequest { ProductName = "Small Item", Price = 30m, Quantity = 1 }
                },
                CouponCode = "FLAT50"
            };

            // Act
            var result = _orderService.CalculateOrder(request);

            // Assert
            // Total = 30
            // Discount = Min(30, 50) = 30
            // Taxable = 0
            // Tax = 0
            // Net = 0
            Assert.Equal(30m, result.TotalAmount);
            Assert.Equal(30m, result.DiscountAmount);
            Assert.Equal(0m, result.TaxAmount);
            Assert.Equal(0m, result.NetAmount);
        }

        [Fact]
        public void CalculateOrder_WithNoCoupon_ShouldHaveZeroDiscount()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                CustomerName = "John Doe",
                Items = new List<OrderItemRequest>
                {
                    new OrderItemRequest { ProductName = "Item 1", Price = 1000m, Quantity = 1 }
                },
                CouponCode = null
            };

            // Act
            var result = _orderService.CalculateOrder(request);

            // Assert
            // Total = 1000
            // Discount = 0
            // Taxable = 1000
            // Tax = 70
            // Net = 1070
            Assert.Equal(1000m, result.TotalAmount);
            Assert.Equal(0m, result.DiscountAmount);
            Assert.Equal(70m, result.TaxAmount);
            Assert.Equal(1070m, result.NetAmount);
        }

        [Theory]
        [InlineData(100.00, 7.00, 107.00)]
        [InlineData(1000.00, 70.00, 1070.00)]
        public void CalculateOrder_TaxCalculation_ShouldBe7PercentExclude(decimal total, decimal expectedTax, decimal expectedNet)
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                CustomerName = "Tax Test",
                Items = new List<OrderItemRequest>
                {
                    new OrderItemRequest { ProductName = "Test", Price = total, Quantity = 1 }
                }
            };

            // Act
            var result = _orderService.CalculateOrder(request);

            // Assert
            Assert.Equal(expectedTax, result.TaxAmount);
            Assert.Equal(expectedNet, result.NetAmount);
        }
    }
}
