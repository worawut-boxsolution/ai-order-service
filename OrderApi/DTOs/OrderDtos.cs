// Redundant DTOs removed to avoid ambiguity with OrderApi.Models
namespace OrderApi.DTOs;

public class OrderResponse
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}
