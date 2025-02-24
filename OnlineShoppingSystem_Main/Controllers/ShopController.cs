using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingSystem_Main.Data.Models;
using OnlineShoppingSystem_Main.Models;
using Service.Interface;

namespace OnlineShoppingSystem_Main.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _context;
        private readonly ILogger<ProductController> _logger;



        public ProductController(IProductService context, ILogger<ProductController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? categoryId, int page = 1)
        {
            int pageSize = 9;
            var categories = await _context.GetCategoriesAsync();

            var products = await _context.GetProductsAsync(categoryId, page, pageSize);
            var totalProducts = products.Count();

            ViewBag.Categories = categories;
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = Math.Ceiling(totalProducts / (double)pageSize);

            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.GetProductDetailsAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
    }
}
