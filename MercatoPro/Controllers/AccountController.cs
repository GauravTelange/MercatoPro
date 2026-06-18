using MercatoPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MercatoPro.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }


        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(User user) {

            if (ModelState.IsValid)
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Email","Email already registered");
                    return View(user);
                }

                user.Role = "Customer";

                user.CreatedDate = DateTime.Now;

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(user);
        }
    }
}
