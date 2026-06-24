using AdminApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminApp.Controllers
{
    public class ProductController : Controller
    {

        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
           _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products.Include(p => p.Category).ToList();
            return View(products);
        }

        public IActionResult Add()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Add(Product product)
        {

            // ModelState.Remove("Category");
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        public IActionResult Edit(int id )
        {
            var product = _context.Products.Find(id);

            if (product == null) return NotFound();

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);

        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                _context.Products.Update(product);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);

            if(product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
               
            }
            return RedirectToAction("Index");
        }



    }
}
