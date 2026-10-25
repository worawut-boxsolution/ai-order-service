using System.ComponentModel.DataAnnotations;

namespace OrderApi.Models;

public class Order
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string CustomerName { get; set; } = string.Empty;
    
    public decimal TotalAmount { get; set; }
    
    public decimal DiscountAmount { get; set; }
    
    public decimal TaxAmount { get; set; }
    
    public decimal NetAmount { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class CreateOrderRequest
{
    [Required]
    public string CustomerName { get; set; } = string.Empty;
    
    public List<OrderItemRequest> Items { get; set; } = new();
    
    public string? CouponCode { get; set; }
}

public class OrderItemRequest
{
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
