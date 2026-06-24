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
        
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id);

            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category) {

            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);

                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(category);

        }

        public IActionResult Delete(int id) {

            var category = _context.Categories.Find(id);

            if (category != null) {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }
 
            return RedirectToAction("Index");


        }
    }
}
