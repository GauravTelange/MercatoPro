using MercatoAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MercatoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        public readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCartItems(int userId)
        {
            var cartItems = await _context.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .ToListAsync();
            return cartItems;
        }

        [HttpPost]
        public async  Task<ActionResult<Cart>> AddToCart(Cart cart)
        {
            var existingItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == cart.UserId && c.ProductId == cart.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += cart.Quantity;
                _context.Carts.Update(existingItem);
            }
            else
            {
                _context.Carts.Add(cart);
            }
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCartItems), new { userId = cart.UserId }, cart);
        }

        [HttpDelete("{cartId}")]
        public async Task<IActionResult> RemoveFromCart(int cartId)
        {
            var cartItem = await _context.Carts.FindAsync(cartId);
            if (cartItem == null)
            {
                return NotFound();
            }
            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }



    }
}
