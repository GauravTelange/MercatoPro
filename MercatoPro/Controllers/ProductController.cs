using MercatoPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MercatoPro.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? categoryId)
        {
            var categories = _context.Categories.ToList();
            var products = categoryId == null
                ? _context.Products.Include(p => p.Category).ToList()
                : _context.Products.Include(p => p.Category)
                                   .Where(p => p.CategoryId == categoryId)
                                   .ToList();

            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = categoryId;

            return View(products);
        }
    }
}
