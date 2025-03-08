using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.GetProductDetailsAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }


            return View(product);


        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int quantity)
        {
            var product = await _context.GetProductDetailsAsync(id);
            if (product == null || quantity <= 0)
            {
                return NotFound();
            }


            return RedirectToAction("Index", "Cart"); 
        }


    }
}
