using OnlineShoppingSystem_Main.Models;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<Product>> GetFeaturedProductsAsync()
        {
            return await _productRepository.GetFeaturedProductsAsync();
        }

        public async Task<List<Product>> GetLatestProductsAsync()
        {
            return await _productRepository.GetLatestProductsAsync();
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllProductsAsync();
        }
        public async Task<IEnumerable<Product>> GetProductsAsync(int? categoryId, int page, int pageSize)
        {
            return await _productRepository.GetProductsAsync(categoryId, page, pageSize);
        }

        public async Task<Product> GetProductDetailsAsync(int productId)
        {
            return await _productRepository.GetProductByIdAsync(productId);
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _productRepository.GetCategoriesAsync();
        }
    }

}
