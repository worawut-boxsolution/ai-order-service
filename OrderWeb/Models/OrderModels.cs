namespace OrderWeb.Models;

public class OrderDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateOrderRequest
{
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
