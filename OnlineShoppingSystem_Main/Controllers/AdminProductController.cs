using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingSystem_Main.Data.Models;
using OnlineShoppingSystem_Main.Models;
using System.Linq;

namespace OnlineShoppingSystem_Main.Controllers
{
    public class AdminProductController : Controller
    {
        private readonly Swd392OssContext _context;

        public AdminProductController(Swd392OssContext context)
        {
            _context = context;
        }

        public IActionResult ProductList(int page = 1, int pageSize = 10)
        {
            var products = _context.Products
                                   .Include(p => p.ProductStatus) 
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToList();

            var totalCount = _context.Products.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.CurrentPage = page;

            return View(products);
        }


       
    }
}
