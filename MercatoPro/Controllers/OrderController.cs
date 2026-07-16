using MercatoPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MercatoPro.Controllers
{
    public class OrderController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrderController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("MercatoAPI");
             
            var response = await client.PostAsync($"api/Order/{userId}", null);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var order = JsonSerializer.Deserialize<Order>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return RedirectToAction("Payment", new { orderId = order.OrderId });
            }
            else
            {
                return RedirectToAction("Index","Cart");
            }

        }

        [HttpGet]
        public async Task<IActionResult> Payment(int orderId)
        {
            
            var client = _httpClientFactory.CreateClient("MercatoAPI");

            var res = await client.GetAsync($"api/Order/{orderId}");

            var json = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
            {
                return Content($"API Error: {res.StatusCode} - {json}");
            }

            var order = JsonSerializer.Deserialize<Order>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(order);
        }
    }
}
