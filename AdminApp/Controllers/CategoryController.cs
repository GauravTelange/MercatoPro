using AdminApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AdminApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CategoryController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        //List Category
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var category = await _httpClientFactory.CreateClient("MercatoAPI").GetAsync("api/Categories");
            var json = await category.Content.ReadAsStringAsync();
            var categories = JsonSerializer.Deserialize<List<Category>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(categories);
        }

        //Add Category
        public IActionResult Add()
        {
            return View();
        }

        //Post Category 
        [HttpPost]
        public async Task<IActionResult> Add(Category category)
        {
            
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            var client = _httpClientFactory.CreateClient("MercatoAPI");

            var json = JsonSerializer.Serialize(category);
            
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await client.PostAsync("api/Categories", content);
            
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while adding the category.");
                return View(category);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("MercatoAPI");
            var response = await client.GetAsync($"api/Categories/{id}");
            var json = await response.Content.ReadAsStringAsync();
            var category = JsonSerializer.Deserialize<Category>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            
            var client = _httpClientFactory.CreateClient("MercatoAPI");


            var json = JsonSerializer.Serialize(category);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await client.PutAsync($"api/Categories/{category.CategoryId}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            } else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the category.");
                return View(category);
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("MercatoAPI");
            
            var response = await client.DeleteAsync($"api/Categories/{id}");
            
            return RedirectToAction("Index");
        }
    }
}
