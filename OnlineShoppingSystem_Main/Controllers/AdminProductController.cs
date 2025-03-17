using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingSystem_Main.Models;
using Service.Implementation;
using Service.Interface;
using System.Linq;
using Api.Implementation;
using Api.Interface;
using Data.Models;

namespace OnlineShoppingSystem_Main.Controllers
{
    public class AdminProductController : Controller
    {
        private readonly IProductService _context;
        private readonly ILogger<HomeController> _logger;
        private readonly ICloudinaryProxy _cloudinaryService;
        private readonly ICategoryService _categoryService;


        public AdminProductController(IProductService context, ILogger<HomeController> logger, ICloudinaryProxy cloudinaryService, ICategoryService categoryService)
        {
            _context = context;
            _logger = logger;
            _cloudinaryService = cloudinaryService;
            _categoryService = categoryService;
        }
        public async Task<IActionResult> ProductList(int page = 1, int pageSize = 10)
        {
            var products = _context.GetPagedProducts(page, pageSize, out int totalCount);

            var categories = await _categoryService.GetCategoriesAsync();
            ViewBag.Categories = categories;
            foreach (var product in products)
            {
                if (product.ProductImages == null)
                {
                    product.ProductImages = new List<ProductImage>();
                }
                if (product.Category == null)
                {
                    product.Category = new Category { CategoryName = "No Category" };
                }
            }

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
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product model, List<IFormFile> ProductImages)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false });
            }

            List<ProductImage> uploadedImages = new List<ProductImage>();
            if (ProductImages != null && ProductImages.Count > 0)
            {
                foreach (var image in ProductImages)
                {
                    string imageUrl = await _cloudinaryService.UploadImageAsync(image);
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        uploadedImages.Add(new ProductImage { ProductImageUrl = imageUrl });
                    }
                }
            }

            bool isAdded = _context.AddProductWithImages(model, uploadedImages);
            return Json(new { success = isAdded });
        }




    }
}
