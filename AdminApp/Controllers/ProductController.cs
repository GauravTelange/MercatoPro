using AdminApp.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AdminApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Cloudinary _cloudinary;

        public ProductController(IHttpClientFactory httpClientFactory, Cloudinary cloudinary)
        {
            _httpClientFactory = httpClientFactory;
            _cloudinary = cloudinary;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("MercatoAPI");

            var response = await client.GetAsync("api/Products");

            var json = await response.Content.ReadAsStringAsync();

            var product = JsonSerializer.Deserialize<List<Product>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var client = _httpClientFactory.CreateClient("MercatoAPI");

            var response = await client.GetAsync("api/Categories");
            var json = await response.Content.ReadAsStringAsync();
            var categories = JsonSerializer.Deserialize<List<Category>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            ViewBag.Categories = categories;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = imageFile.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(imageFile.FileName, stream),
                    Folder = "mercato-products"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    ModelState.AddModelError(string.Empty, "Image upload failed: " + uploadResult.Error.Message);
                }
                else
                {
                    product.ImageUrl = uploadResult.SecureUrl.ToString();
                }
            }

            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("MercatoAPI");
                var json = JsonSerializer.Serialize(product);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/Products", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while adding the product.");
                }
            }
            var catClient = _httpClientFactory.CreateClient("MercatoAPI");
            var catResponse = await catClient.GetAsync("api/Categories");
            var catJson = await catResponse.Content.ReadAsStringAsync();
            var categories = JsonSerializer.Deserialize<List<Category>>(catJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            ViewBag.Categories = categories;
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _httpClientFactory.CreateClient("MercatoAPI").GetAsync($"api/Products/{id}");
            if (!product.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var json = await product.Content.ReadAsStringAsync();
            var productData = JsonSerializer.Deserialize<Product>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var categoriesResponse = await _httpClientFactory.CreateClient("MercatoAPI").GetAsync("api/Categories");
            var categoriesJson = await categoriesResponse.Content.ReadAsStringAsync();
            var categories = JsonSerializer.Deserialize<List<Category>>(categoriesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            ViewBag.Categories = categories;

            return View(productData);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product, IFormFile? imageFile)
        {
            ModelState.Remove("Category");
            ModelState.Remove("ImageFile");

            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = imageFile.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(imageFile.FileName, stream),
                    Folder = "mercato-products"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    ModelState.AddModelError(string.Empty, "Image upload failed: " + uploadResult.Error.Message);
                }
                else
                {
                    product.ImageUrl = uploadResult.SecureUrl.ToString();
                }
            }

            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("MercatoAPI");
                var json = JsonSerializer.Serialize(product);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"api/Products/{product.ProductId}", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the product.");
                }
            }
            var catClient = _httpClientFactory.CreateClient("MercatoAPI");
            var catResponse = await catClient.GetAsync("api/Categories");
            var catJson = await catResponse.Content.ReadAsStringAsync();
            var categories = JsonSerializer.Deserialize<List<Category>>(catJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            ViewBag.Categories = categories;

            return View(product);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("MercatoAPI");
            await client.DeleteAsync($"api/Products/{id}");

            return RedirectToAction("Index");
        }
    }
}