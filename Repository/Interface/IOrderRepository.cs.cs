using Data.Models;
using Data.Models.GHN;
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
        Task<bool> ConfirmOrderAsync(int orderId, int confirmStatus);

		// Track Order Detail
		Task<List<Order>> GetOrdersByUserIdAsync(string userId);

        //Task<bool> CancelOrderAsync(int orderId);
        Task<bool> CancelOrderAsync(string orderCode);

        Task<Order> GetOrderDetailsAsync(int orderId);
        Task<bool> UpdateOrderAsync(Order order);

        Task<GhnOrderDetailResponse> GetOrderDetailsFromGhnAsync(string orderCode);
    }

}