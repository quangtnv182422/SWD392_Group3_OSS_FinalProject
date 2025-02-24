﻿using OnlineShoppingSystem_Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IProductRepository
    {
        Task<List<Product>> GetFeaturedProductsAsync();
        Task<List<Product>> GetLatestProductsAsync();
        Task<List<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetProductsAsync(int? categoryId, int page, int pageSize);
        Task<Product> GetProductByIdAsync(int productId);
        Task<IEnumerable<Category>> GetCategoriesAsync();
    }

}
