using MercatoPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MercatoPro.Controllers
{
    public class CartController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CartController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public  async Task<IActionResult> Index()
        {
            var  userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null) 
            { 
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("MercatoAPI");

            var response = await client.GetAsync($"api/Cart/{userId}");
            var json = await response.Content.ReadAsStringAsync();
            var cartItems = JsonSerializer.Deserialize<List<Cart>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(cartItems);



        }

        [HttpPost]
        public async Task<ActionResult> Remove(int cartId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var client = _httpClientFactory.CreateClient("MercatoAPI");
            var respone = await client.DeleteAsync($"api/Cart/{cartId}");
            
            return RedirectToAction("index");
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null) {

                return RedirectToAction("Login", "Account");
            }
            

            var client = _httpClientFactory.CreateClient("MercatoAPI");

            var cartItem = new Cart
            {
                UserId = userId.Value,
                ProductId = productId,
                Quantity = 1
            };

            var json = JsonSerializer.Serialize(cartItem);

            var response = await client.PostAsync("api/Cart", new StringContent(json, System.Text.Encoding.UTF8, "application/json"));


            return RedirectToAction("Index");
        }
    }
}
