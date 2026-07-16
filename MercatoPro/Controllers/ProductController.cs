using MercatoPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MercatoPro.Controllers
{
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;


        public ProductController(IHttpClientFactory context)
        {
            _httpClientFactory = context;
        }
        public  async Task<IActionResult> Index(int? categoryId)
        {
            var client = _httpClientFactory.CreateClient("MercatoAPI");


            var response = await client.GetAsync("api/Products");
            var json = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<List<Product>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }); 
                

            var res = await client.GetAsync("api/Categories");
            var jsonCat = await res.Content.ReadAsStringAsync();
            var categories = JsonSerializer.Deserialize<List<Category>>(jsonCat, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = categoryId;

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value).ToList();
            }

            return View(products);
        


        }
    }
}
