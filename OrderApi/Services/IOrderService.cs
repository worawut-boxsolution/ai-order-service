namespace OrderApi.Services;

public interface IOrderService
{
    decimal CalculateNetAmount(decimal total, decimal discount);
}
