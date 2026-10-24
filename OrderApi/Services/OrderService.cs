namespace OrderApi.Services;

public class OrderService : IOrderService
{
    private const decimal VatRate = 0.07m;

    public decimal CalculateNetAmount(decimal total, decimal discount)
    {
        // คำนวณ: (ยอดรวม - ส่วนลด) + VAT 7%
        var afterDiscount = total - discount;
        var vatAmount = afterDiscount * VatRate;
        return afterDiscount + vatAmount;
    }
}
