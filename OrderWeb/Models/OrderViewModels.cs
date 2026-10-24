using System.ComponentModel.DataAnnotations;

namespace OrderWeb.Models
{
    public class OrderItemViewModel
    {
        [Required]
        public string ProductName { get; set; } = string.Empty;
        
        [Range(0.01, 1000000)]
        public decimal Price { get; set; }
        
        [Range(1, 100)]
        public int Quantity { get; set; } = 1;
    }

    public class CreateOrderViewModel
    {
        [Required]
        public string CustomerName { get; set; } = string.Empty;
        public List<OrderItemViewModel> Items { get; set; } = new();
        public string? CouponCode { get; set; }
    }

    public class OrderResponse
    {
        public string Id { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal NetAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
