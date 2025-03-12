using Data.Models;
using OnlineShoppingSystem_Main.Models;
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
        List<Product> GetPagedProducts(int page, int pageSize, out int totalCount);
        Product GetProductById(int productId);
        void UpdateProduct(Product product);
        bool AddProductWithImages(Product product, List<ProductImage> productImages);
        void RemoveProduct(int productId);
        bool UpdateProductWithImages(Product product, List<ProductImage> newImages);
        Task<ProductStatus?> GetProductStatusByIdAsync(int productStatusId);
        Task<List<ProductStatus>> GetProductStatusesAsync();
        void RemoveProductImages(int id);

    }

}
