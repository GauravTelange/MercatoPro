using MercatoPro.Models;
using Microsoft.AspNetCore.Mvc;

namespace MercatoPro.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null) {

                return RedirectToAction("Login", "Account");
            }

            var cartItem = new Cart
            {
                UserId = userId.Value,
                ProductId = productId,
                Quantity = 1
            };

            _context.Carts.Add(cartItem);
            _context.SaveChanges();


            return RedirectToAction("Index","Product");
        }
    }
}
