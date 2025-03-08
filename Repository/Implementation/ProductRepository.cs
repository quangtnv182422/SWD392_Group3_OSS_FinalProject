using Data.Models;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingSystem_Main.Models;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implementation
{
    public class ProductRepository : IProductRepository
    {
        private readonly Swd392OssContext _context;

        public ProductRepository(Swd392OssContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetFeaturedProductsAsync()
        {
            return await _context.Products
                .Include(p => p.ProductImages)
                .Where(p => p.IsFeatured && p.ProductStatusId == 1)
                .Take(6)
                .ToListAsync();
        }

        public async Task<List<Product>> GetLatestProductsAsync()
        {
            return await _context.Products
                .Include(p => p.ProductImages)
                .Where(p => p.ProductStatusId == 1)
                .OrderBy(p => p.CreatedAt)
                .Take(5)
                .ToListAsync();
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.ProductImages)
                .Where(p => p.ProductStatusId == 1)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetProductsAsync(int? categoryId, int page, int pageSize)
        {
            var products = _context.Products
                                   .Where(p => p.ProductStatusId == 1)
                                   .Include(p => p.Category)
                                   .Include(p => p.ProductImages)
                                   .AsQueryable();

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            return await products.Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                .Where(p => p.ProductId == productId && p.ProductStatusId == 1) 
                .Include(p => p.ProductImages) 
                .Include(p => p.ProductStatus)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }
        public List<Product> GetPagedProducts(int page, int pageSize, out int totalCount)
        {
            totalCount = _context.Products.Count();
            return _context.Products
                           .Include(p => p.ProductStatus)
                           .Include(p => p.ProductImages)
                           .Skip((page - 1) * pageSize)
                           .Take(pageSize)
                           .ToList();
        }

        public Product GetProductById(int productId)
        {
            return _context.Products
                           .Include(p => p.ProductImages)
                           .FirstOrDefault(p => p.ProductId == productId);

        }

        public void UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }
        public bool AddProductWithImages(Product product, List<ProductImage> productImages)
        {
            if (product.ProductStatusId == 0) 
            {
                product.ProductStatusId = 1; 
            }

            _context.Products.Add(product);
            _context.SaveChanges();  

            foreach (var image in productImages)
            {
                image.ProductId = product.ProductId; 
                _context.ProductImages.Add(image);  
            }

            return _context.SaveChanges() > 0; 
        }

    }

}
