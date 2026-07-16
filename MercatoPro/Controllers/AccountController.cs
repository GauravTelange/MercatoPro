using MercatoPro.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MercatoPro.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }


        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("MercatoAPI");
                user.Role = "Customer";
                user.CreatedDate = DateTime.Now;

                var json = JsonSerializer.Serialize(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/User/register", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("Email", "Email already registered");
                    return View(user);
                }
            }
            return View(user);
        }
        public IActionResult Login()
        {
            return View();
        }
        

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {

            var loginData = new LoginRequest { Email = email, Password = password };

            string json = JsonSerializer.Serialize(loginData);

            var content= new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient("MercatoAPI");

            var response = await client.PostAsync("api/User/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<User>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                HttpContext.Session.SetString("UserName", user.FullName);
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetInt32("UserId", user.UserId);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View();
            }

            
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

            [HttpPost]
            public async Task<IActionResult> ForgotPassword(string email, string newPassword, string confirmPassword)
            {
                if (newPassword != confirmPassword)
                {
                    TempData["Error"] = "Passwords do not match!";
                    return View();
                }

                var res = new ResetPasswordDTO
                {
                    Email = email,
                    NewPassword = newPassword
                };

                var json = JsonSerializer.Serialize(res);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var client = _httpClientFactory.CreateClient("MercatoAPI");
                var response = await client.PutAsync("api/User/resetpassword", content);


                if(response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Password reset successful!";
                    return RedirectToAction("Login");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = errorMessage;
                    return View();
                }
            }
    }

    public class ResetPasswordDTO
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }

    
}
