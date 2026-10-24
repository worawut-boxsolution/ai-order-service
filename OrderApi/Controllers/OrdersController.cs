using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderApi.Data;
using OrderApi.DTOs;
using OrderApi.Models;
using OrderApi.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly IOrderService _orderService;

        public OrdersController(OrderDbContext context, IOrderService orderService)
        {
            _context = context;
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetOrders()
        {
            var orders = await _context.Orders.OrderByDescending(o => o.CreatedAt).ToListAsync();
            
            var response = orders.Select(o => new OrderResponse
            {
                Id = o.Id,
                CustomerName = o.CustomerName,
                TotalAmount = o.TotalAmount,
                DiscountAmount = o.DiscountAmount,
                TaxAmount = o.TaxAmount,
                NetAmount = o.NetAmount,
                CreatedAt = o.CreatedAt
            });

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponse>> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.CustomerName))
            {
                return BadRequest("Invalid order request.");
            }

            var order = _orderService.CalculateOrder(request);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var response = new OrderResponse
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                TotalAmount = order.TotalAmount,
                DiscountAmount = order.DiscountAmount,
                TaxAmount = order.TaxAmount,
                NetAmount = order.NetAmount,
                CreatedAt = order.CreatedAt
            };

            return CreatedAtAction(nameof(GetOrders), new { id = order.Id }, response);
        }
    }
}
