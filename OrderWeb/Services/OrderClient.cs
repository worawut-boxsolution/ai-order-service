using OrderWeb.Models;
using System.Net.Http.Json;

namespace OrderWeb.Services
{
    public class OrderClient
    {
        private readonly HttpClient _httpClient;

        public OrderClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<OrderResponse>> GetOrdersAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<OrderResponse>>("api/orders") ?? new();
        }

        public async Task<OrderResponse?> CreateOrderAsync(CreateOrderViewModel order)
        {
            var response = await _httpClient.PostAsJsonAsync("api/orders", order);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<OrderResponse>();
            }
            return null;
        }
    }
}
