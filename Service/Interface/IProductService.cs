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
    }

}
