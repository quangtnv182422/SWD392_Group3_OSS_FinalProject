﻿using Data.Models;
using OnlineShoppingSystem_Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IProductService
    {
        Task<List<Product>> GetFeaturedProductsAsync();
        Task<List<Product>> GetLatestProductsAsync();
        Task<List<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetProductsAsync(int? categoryId, int page, int pageSize);
        Task<Product> GetProductDetailsAsync(int productId);
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Product GetProductById(int productId);
        List<Product> GetPagedProducts(int page, int pageSize, out int totalCount);
        bool ChangeProductStatus(int productId, int statusId);

        bool AddProductWithImages(Product product, List<ProductImage> productImages);

        void RemoveProduct(int productId);

        void UpdateProduct(Product product);
        bool UpdateProductWithImages(Product product, List<ProductImage> newImages);
        Task<ProductStatus?> GetProductStatusByIdAsync(int productStatusId);
        Task<List<ProductStatus>> GetProductStatusesAsync();

        void RemoveProductImages(int id);
        Task UpdateProductQuantityAfterOrder(ICollection<OrderItem> orderItem);

	}

}
