using MercatoPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MercatoPro.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null) {
                return RedirectToAction("Login","Account");
            }

            var cartItems = _context.Carts.Include(c => c.Product).Where(c => c.UserId == userId.Value).ToList(); 

            return View(cartItems);
        }

        [HttpPost]
        public IActionResult Remove(int cartId)
        {
            var cartItem = _context.Carts.Find(cartId);

            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                _context.SaveChanges();

            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null) {

                return RedirectToAction("Login", "Account");
            }
            var existingitem = _context.Carts.FirstOrDefault(c=> c.UserId == userId && c.ProductId == productId);

            if (existingitem != null)
            {
                existingitem.Quantity += 1;
                _context.Carts.Update(existingitem);
            }
            else
            {
                var cartItem = new Cart
                {
                    UserId = userId.Value,
                    ProductId = productId,
                    Quantity = 1
                };

                _context.Carts.Add(cartItem);
            }
           

           
            _context.SaveChanges();


            return RedirectToAction("Index","Product");
        }
    }
}
