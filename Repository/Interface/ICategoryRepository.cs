using OnlineShoppingSystem_Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
  

    namespace Api.Interface
    {
        public interface ICategoryRepository
        {
            Task<List<Category>> GetCategoriesAsync(); 
            Task<Category> GetCategoryByIdAsync(int categoryId); 
            Task<bool> AddCategoryAsync(Category category); 
            Task<bool> UpdateCategoryAsync(Category category); 
            Task<bool> DeleteCategoryAsync(int categoryId); 
        }
    }

}
