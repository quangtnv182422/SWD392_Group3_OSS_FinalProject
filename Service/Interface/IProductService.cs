using Data.Models;
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

        List<Product> GetPagedProducts(int page, int pageSize, out int totalCount);
        bool ChangeProductStatus(int productId, int statusId);

        bool AddProductWithImages(Product product, List<ProductImage> productImages);


    }

}
