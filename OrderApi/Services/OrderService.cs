using OrderApi.Models;
using OrderApi.Data;
using Microsoft.EntityFrameworkCore;

namespace OrderApi.Services;

public class OrderService : IOrderService
{
    private readonly OrderDbContext _context;

    public OrderService(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        // 1. Calculate Total Amount
        decimal totalAmount = request.Items.Sum(i => i.Price * i.Quantity);
        
        if (totalAmount < 0)
            throw new ArgumentException("Total amount cannot be negative.");

        // 2. Calculate Discount
        decimal discountAmount = 0;
        if (!string.IsNullOrEmpty(request.CouponCode))
        {
            string coupon = request.CouponCode.ToUpper();
            if (coupon == "SAVE10")
            {
                discountAmount = totalAmount * 0.10m;
            }
            else if (coupon == "FLAT50")
            {
                discountAmount = 50m;
            }
        }
        
        // Ensure discount does not exceed total
        if (discountAmount > totalAmount) discountAmount = totalAmount;

        // 3. Calculate Tax (7% VAT Exclude)
        decimal amountAfterDiscount = totalAmount - discountAmount;
        decimal taxAmount = Math.Round(amountAfterDiscount * 0.07m, 2);

        // 4. Calculate Net Amount
        decimal netAmount = amountAfterDiscount + taxAmount;

        var order = new Order
        {
            CustomerName = request.CustomerName,
            TotalAmount = totalAmount,
            DiscountAmount = discountAmount,
            TaxAmount = taxAmount,
            NetAmount = netAmount,
            CreatedAt = DateTime.UtcNow
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return order;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders.OrderByDescending(o => o.CreatedAt).ToListAsync();
    }
}
