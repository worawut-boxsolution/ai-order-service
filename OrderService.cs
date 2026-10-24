using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Domain.Models
{
    public record OrderItem
    {
        public required string ProductId { get; init; }
        public required string ProductName { get; init; }
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
    }

    public enum DiscountType
    {
        FixedAmount,
        Percentage
    }

    public record DiscountRequest
    {
        public DiscountType Type { get; init; }
        public decimal Value { get; init; }
    }

    public class OrderCalculationResult
    {
        public decimal Subtotal { get; set; }        // ราคารวมสินค้าก่อนหักส่วนลด
        public decimal TotalDiscount { get; set; }   // จำนวนเงินส่วนลดรวม
        public decimal AmountAfterDiscount { get; set; } // ราคาหลังหักส่วนลด (ฐานภาษี)
        public decimal VatAmount { get; set; }       // ภาษีมูลค่าเพิ่ม
        public decimal NetTotal { get; set; }        // ยอดรวมสุทธิที่ต้องชำระ
        public decimal VatRate { get; init; }        // อัตราภาษีที่ใช้คำนวณ
    }
}

namespace App.Services
{
    using App.Domain.Models;

    public interface IOrderService
    {
        /// <summary>
        /// คำนวณราคาสินค้า ส่วนลด และภาษีมูลค่าเพิ่ม
        /// </summary>
        OrderCalculationResult CalculateOrder(List<OrderItem> items, DiscountRequest? discount = null);
    }

    public class OrderService : IOrderService
    {
        private const decimal VAT_RATE = 0.07m;

        public OrderCalculationResult CalculateOrder(List<OrderItem> items, DiscountRequest? discount = null)
        {
            if (items == null || !items.Any())
                throw new ArgumentException("Order items cannot be empty.");

            // Validation for Quantity and UnitPrice
            foreach (var item in items)
            {
                if (item.Quantity <= 0)
                    throw new ArgumentException($"Invalid quantity for product {item.ProductId}. Quantity must be greater than zero.");
                
                if (item.UnitPrice < 0)
                    throw new ArgumentException($"Invalid unit price for product {item.ProductId}. Unit price cannot be negative.");
            }

            var result = new OrderCalculationResult
            {
                VatRate = VAT_RATE
            };

            // 1. Calculate Subtotal (Price * Quantity)
            result.Subtotal = items.Sum(item => item.UnitPrice * item.Quantity);

            // 2. Calculate Discount
            result.TotalDiscount = CalculateDiscountAmount(result.Subtotal, discount);

            // 3. Calculate Amount After Discount (Tax Base)
            // ป้องกันราคาติดลบในกรณีส่วนลดมากกว่าราคาสินค้า
            result.AmountAfterDiscount = Math.Max(0, result.Subtotal - result.TotalDiscount);

            // 4. Calculate VAT 7%
            // ใช้ MidpointRounding.AwayFromZero เพื่อความถูกต้องตามหลักบัญชี
            result.VatAmount = Math.Round(result.AmountAfterDiscount * result.VatRate, 2, MidpointRounding.AwayFromZero);

            // 5. Calculate Net Total
            result.NetTotal = result.AmountAfterDiscount + result.VatAmount;

            return result;
        }

        private decimal CalculateDiscountAmount(decimal subtotal, DiscountRequest? discount)
        {
            if (discount == null || discount.Value <= 0) return 0m;

            return discount.Type switch
            {
                DiscountType.Percentage => Math.Round(subtotal * (discount.Value / 100m), 2, MidpointRounding.AwayFromZero),
                DiscountType.FixedAmount => discount.Value,
                _ => 0m
            };
        }
    }
}
