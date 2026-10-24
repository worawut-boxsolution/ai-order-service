using OrderApi.DTOs;
using OrderApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderApi.Services
{
    public class OrderService : IOrderService
    {
        public Order CalculateOrder(CreateOrderRequest request)
        {
            // 1. Total Amount (Subtotal)
            decimal totalAmount = request.Items.Sum(item => item.Price * item.Quantity);

            // 2. Discount Amount
            decimal discountAmount = CalculateDiscount(totalAmount, request.CouponCode);

            // 3. Taxable Amount
            decimal taxableAmount = totalAmount - discountAmount;

            // 4. Tax Amount (VAT 7% Exclude)
            decimal taxAmount = taxableAmount * 0.07m;

            // 5. Net Amount
            decimal netAmount = taxableAmount + taxAmount;

            return new Order
            {
                CustomerName = request.CustomerName,
                TotalAmount = totalAmount,
                DiscountAmount = discountAmount,
                TaxAmount = taxAmount,
                NetAmount = netAmount,
                CreatedAt = DateTime.UtcNow
            };
        }

        private decimal CalculateDiscount(decimal total, string? couponCode)
        {
            if (string.IsNullOrWhiteSpace(couponCode)) return 0;

            return couponCode.ToUpper() switch
            {
                "SAVE10" => total * 0.10m,
                "FLAT50" => Math.Min(total, 50m),
                _ => 0
            };
        }
    }
}
