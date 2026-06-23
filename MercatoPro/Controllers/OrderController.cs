using MercatoPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult PlaceOrder()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = _context.Carts.Include(c => c.Product).Where(c => c.UserId == userId.Value).ToList();

            if (!cartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            decimal Total = cartItems.Sum(c => c.Product.Price * c.Quantity);

            var Order = new Order
            {
                UserId = userId.Value,
                TotalAmount = Total,
                OrderStatus = "Pending",
                OrderDate = DateTime.Now
            };

            _context.Orders.Add(Order);
            _context.SaveChanges();

            foreach (var item in cartItems)
            {

                var orderItem = new OrderItem
                {
                    OrderId = Order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                };
                _context.OrderItems.Add(orderItem);
            }
            _context.Carts.RemoveRange(cartItems);
            _context.SaveChanges();

            return RedirectToAction("Payment", new { orderId = Order.OrderId });

        }

        public IActionResult Payment(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems).ThenInclude(o => o.Product)
                .FirstOrDefault(o => o.OrderId == orderId);

            return View(order);
        }
    }
}
