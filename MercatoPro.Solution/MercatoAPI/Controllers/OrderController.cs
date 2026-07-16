using MercatoAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace MercatoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        public readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost("{userId}")]
        public async Task<ActionResult<Order>> PlaceOrder(int userId)
        {
            var cartItems = await _context.Carts.Include(c => c.Product).Where(c => c.UserId == userId).ToListAsync();

            if (cartItems.Count == 0)
            {
                return BadRequest("Cart is Empty");
            }

            var order = new Order();

            order.UserId = userId;
            order.TotalAmount = cartItems.Sum(c => c.Product.Price * c.Quantity);
            order.OrderDate = DateTime.Now;
            _context.Orders.Add(order); ;

            await _context.SaveChangesAsync();

            foreach (var item in cartItems) {

                var orderItem = new OrderItem();
                
                orderItem.ProductId = item.ProductId;
                orderItem.OrderId = order.OrderId;
                orderItem.Quantity = item.Quantity;
                orderItem.Price = item.Product.Price;
                
                _context.Add(orderItem); 
            
            }

            _context.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            return Ok(order);

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders(int userId)
        {
            var orders = await _context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product).Where(o => o.UserId == userId).ToListAsync();

            if (orders.Count == 0)
            {
                return NotFound("No orders found for this user.");
            }

            return orders;
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<Order>> GetOrderById(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound("Order not found.");
            }

            return Ok(order);
        }
    }
}
