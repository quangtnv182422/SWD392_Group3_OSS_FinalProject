using Data.Models;
using Microsoft.EntityFrameworkCore;
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
        public List<Product> GetPagedProducts(int page, int pageSize, out int totalCount)
        {
            return _productRepository.GetPagedProducts(page, pageSize, out totalCount);
        }

        public bool ChangeProductStatus(int productId, int statusId)
        {
            var product = _productRepository.GetProductById(productId);
            if (product == null)
                return false;

            if (statusId == 3)
                product.Quantity = 0;

            product.ProductStatusId = statusId;
            _productRepository.UpdateProduct(product);

            return true;
        }

    

        public bool AddProductWithImages(Product product, List<ProductImage> productImages)
        {
            return _productRepository.AddProductWithImages(product, productImages);
        }

        public void RemoveProduct(int productId)
        {
            _productRepository.RemoveProduct(productId);
        }

        public void UpdateProduct(Product product)
        {
            _productRepository.UpdateProduct(product);
        }

        public bool UpdateProductWithImages(Product product, List<ProductImage> newImages)
        {
            return _productRepository.UpdateProductWithImages(product, newImages);
        }
        public async Task<ProductStatus?> GetProductStatusByIdAsync(int productStatusId)
        {
            return await _productRepository.GetProductStatusByIdAsync(productStatusId);
        }

        public async Task<List<ProductStatus>> GetProductStatusesAsync()
        {
            return await _productRepository.GetProductStatusesAsync();
        }
        public Product GetProductById(int id)
        {
            return _productRepository.GetProductById(id);
        }
        public void RemoveProductImages(int id)
        {
             _productRepository.RemoveProductImages(id);
        }
    }

}
