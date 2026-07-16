using AdminApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AdminApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpclientfactory;

        public AccountController(IHttpClientFactory httpclientfactory)
        {
            _httpclientfactory = httpclientfactory;
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // 1. LoginRequest object banao (Email, Password)
            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password
            };

            var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(loginRequest), System.Text.Encoding.UTF8, "application/json");

            var client = _httpclientfactory.CreateClient("MercatoAPI");
            var response = await client.PostAsync("/api/User/login", jsonContent);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var user = System.Text.Json.JsonSerializer.Deserialize<User>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (user != null && user.Role == "Admin")
                {
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetString("UserRole", user.Role);
                    HttpContext.Session.SetString("AdminName", user.FullName);   

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Access denied, admin only");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password");
            }
            return View(loginRequest);
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Logout() { 
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
