namespace OrderApi.Services;

using OrderApi.DTOs;
using OrderApi.Models;

public interface IOrderService
{
    Order CalculateOrder(CreateOrderRequest request);
}
