using AdminApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdminApp.Controllers
{
    public class CategoryController : Controller
    {
        public readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        //List Category
        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        //Add Category
        public IActionResult Add()
        {
            return View();
        }

        //Post Category 
        [HttpPost]
        public IActionResult Add(Category category)
        {
            if (ModelState.IsValid) {
                _context.Categories.Add(category);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }
        
    }
}
