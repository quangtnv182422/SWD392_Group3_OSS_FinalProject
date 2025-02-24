using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingSystem_Main.Data.Models;
using OnlineShoppingSystem_Main.Models;

namespace OnlineShoppingSystem_Main.Controllers
{
    public class ProductController : Controller
    {
        private readonly Swd392OssContext _context;

        public ProductController(Swd392OssContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();

            var products = _context.Products
                                   .Include(p => p.Category)
                                   .Include(p => p.ProductImages)
                                   .ToList();

            ViewBag.Categories = categories;

            return View(products); 
        }

      


        public IActionResult Details(int id)
        {
            var product = _context.Products
                                 .Include(p => p.Category)
                                 .Include(p => p.ProductImages)
                                 .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
