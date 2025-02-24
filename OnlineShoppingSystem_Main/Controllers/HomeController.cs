using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineShoppingSystem_Main.Data.Models;
using OnlineShoppingSystem_Main.Models;
using System.Diagnostics;

namespace OnlineShoppingSystem_Main
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Swd392OssContext _context;

        public HomeController(ILogger<HomeController> logger, Swd392OssContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var featuredProducts = await _context.Products
         .Include(p => p.ProductImages)
         .Where(p => p.IsFeatured && p.ProductStatusId == 1) 
         .Take(6)
         .ToListAsync();

            var latestProducts = await _context.Products
                .Include(p => p.ProductImages)
                .Where(p => p.ProductStatusId == 1) 
                .OrderBy(p => p.CreatedAt)
                .Take(5)
                .ToListAsync();

            var allProducts = await _context.Products
                .Include(p => p.ProductImages)
                .Where(p => p.ProductStatusId == 1) 
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            ViewBag.FeaturedProducts = featuredProducts ?? new List<Product>();
            ViewBag.LatestProducts = latestProducts ?? new List<Product>();

            return View(allProducts);
        }

            [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
