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
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ICategoryService _categoryService;


        public AdminProductController(IProductService context, ILogger<HomeController> logger, ICloudinaryService cloudinaryService, ICategoryService categoryService)
        {
            _context = context;
            _logger = logger;
            _cloudinaryService = cloudinaryService;
            _categoryService = categoryService;
        }
        public async Task<IActionResult> ProductList(int page = 1, int pageSize = 10)
        {
            var products = _context.GetPagedProducts(page, pageSize, out int totalCount);
            var productStatuses = await _context.GetProductStatusesAsync();
            var categories = await _categoryService.GetCategoriesAsync();
            ViewBag.Categories = categories;
            ViewBag.ProductStatuses = productStatuses;
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

            model.CreatedAt = DateTime.Now;
            model.ProductStatusId = 1;
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
        [HttpPost]
        public async Task<IActionResult> UpdateProduct(Product product, List<IFormFile> ProductImages)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model State Errors:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return Json(new { success = false, message = "Invalid data" });
            }

            Console.WriteLine($"Product Status ID: {product.ProductStatusId}");

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

            bool isUpdated = _context.UpdateProductWithImages(product, uploadedImages);
            return Json(new { success = isUpdated });
        }


        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                var product = _context.GetProductById(id);

                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found" });
                }

                if (product.Quantity > 0)
                {
                    return Json(new { success = false, message = "Product quantity must be 0 to delete" });
                }

                foreach (var image in product.ProductImages)
                {
                    _cloudinaryService.DeleteImage(image.ProductImageUrl);
                }

                _context.RemoveProductImages(id);

                _context.RemoveProduct(id);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting product with ID {id}: {ex.Message}");
                return Json(new { success = false, message = "Error occurred while deleting product." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var product = await _context.GetProductDetailsAsync(id);
                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found" });
                }

                string categoryName = "No Category";
                if (product.CategoryId.HasValue)
                {
                    var category = await _categoryService.GetCategoryByIdAsync(product.CategoryId.Value);
                    if (category != null)
                    {
                        categoryName = category.CategoryName;
                    }
                }
                string productStatus = "Unknown";
                if (product.ProductStatusId.HasValue)
                {
                    var status = await _context.GetProductStatusByIdAsync(product.ProductStatusId.Value);
                    if (status != null)
                    {
                        productStatus = status.StatusDescription;
                    }
                }

                var productDto = new
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName ?? "N/A",
                    Description = product.Description ?? "N/A",
                    Price = product.Price,
                    SalePrice = product.SalePrice ?? 0,
                    Quantity = product.Quantity,
                    CategoryId = product.CategoryId,
                    CategoryName = categoryName,
                    ProductStatusId = product.ProductStatusId, 
                    ProductStatusDescription = productStatus,
                    IsFeatured = product.IsFeatured,
                    ProductImages = product.ProductImages?.Select(img => img.ProductImageUrl).ToList() ?? new List<string>()
                };

                return Json(new { success = true, product = productDto });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching product: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }








    }
}
