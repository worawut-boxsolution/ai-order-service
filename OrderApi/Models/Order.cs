using System.ComponentModel.DataAnnotations;

namespace OrderApi.Models;

public class Order
{
    public int Id { get; set; }
    
    [Required]
    public string CustomerName { get; set; } = string.Empty;
    
    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
    public decimal NetAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
