using Data.Models;
using OnlineShoppingSystem_Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderByIdAsync(string orderId);
        Task<List<CartItem>> GetCartItemsByIdsAsync(List<int> cartItemIds);
        Task CreateOrderAsync(Order order);
        Task SaveChangesAsync();
    }

}