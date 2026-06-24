using AdminApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdminApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password && u.Role == "Admin");

            if (user != null)
            {
                HttpContext.Session.SetString("Username", user.FullName);
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetInt32("UserId", user.UserId);
                return RedirectToAction("Index", "Home");


            }
            ModelState.AddModelError("", "Ivalid Email or Password");
            return View();

        }
        public IActionResult Logout() { 
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
