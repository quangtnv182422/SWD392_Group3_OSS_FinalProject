using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingSystem_Main.Data.Models;
using OnlineShoppingSystem_Main.Models;
using Service.Interface;
using System.Linq;

namespace OnlineShoppingSystem_Main.Controllers
{
    public class AdminProductController : Controller
    {
        private readonly IProductService _context;
        private readonly ILogger<HomeController> _logger;

        public AdminProductController(IProductService context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult ProductList(int page = 1, int pageSize = 10)
        {
            var products = _context.GetPagedProducts(page, pageSize, out int totalCount);

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.CurrentPage = page;

            return View(products);
        }

        [HttpPost]
        public IActionResult ChangeProductStatus(int productId, int statusId)
        {
            bool result = _context.ChangeProductStatus(productId, statusId);
            return Json(new { success = result });
        }

    }
}
