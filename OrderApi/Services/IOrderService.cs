using OrderApi.Models;

namespace OrderApi.Services;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(CreateOrderRequest request);
    Task<IEnumerable<Order>> GetAllOrdersAsync();
}
