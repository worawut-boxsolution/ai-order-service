using System.Net.Http.Json;
using OrderWeb.Models;

namespace OrderWeb.Services;

public interface IOrderHttpClient
{
    Task<List<OrderResponse>> GetOrdersAsync();
    Task<OrderResponse?> CreateOrderAsync(CreateOrderRequest request);
}

public class OrderHttpClient : IOrderHttpClient
{
    private readonly HttpClient _httpClient;

    public OrderHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<OrderResponse>> GetOrdersAsync()
    {
        try 
        {
            return await _httpClient.GetFromJsonAsync<List<OrderResponse>>("api/orders") ?? new();
        }
        catch 
        {
            return new List<OrderResponse>();
        }
    }

    public async Task<OrderResponse?> CreateOrderAsync(CreateOrderRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/orders", request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<OrderResponse>();
        }
        return null;
    }
}
