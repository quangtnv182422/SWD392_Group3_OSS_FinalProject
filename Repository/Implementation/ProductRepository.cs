using Microsoft.EntityFrameworkCore;
using OnlineShoppingSystem_Main.Data.Models;
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
    }

}
