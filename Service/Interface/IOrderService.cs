using Data.Models;
using Data.Models.GHN;
using Microsoft.AspNetCore.Identity;
using OnlineShoppingSystem_Main.Models;

namespace Service.Interface
{
    public interface IOrderService
    {
        Task<Order> GetOrderByIdAsync(string orderId);
        Task<OrderConfirmationViewModel> CreateOrderConfirmationViewModelAsync(List<int> selectedCartItemIds, AspNetUser currentUser);

        //Checkout Order
        Task<Order> CreateOrderAsync(string fullName,string? customerId,/*string staffId,*/string email, string mobile,string address,string paymentMethod,List<int> cartItemIds,float totalCost,int orderStatus,string? note);
        Task SendOrderConfirmEmail(string email, string fullName, string address, string phoneNumber, string? orderNotes, List<OrderItem> products, string returnLink, string paymentMethod);
        Task<bool> ConfirmOrderAsync(int orderId, int confirmStatus);
        Task<bool> UpdateOrderCodeGHNAsync(Order order, string? orderCodeGHN);
        //---------------
        Task<Order> SaveOrderAsync(Order order);


        // Track Order Detail
        Task<List<Order>> GetOrdersByUserIdAsync(string userId);
        //Task<bool> CancelOrderAsync(int orderId);
        Task<bool> CancelOrderAsync(string orderCode);

        Task<Order> GetOrderDetailsAsync(int orderId);
        Task<bool> UpdateOrderAsync(Order order);

        Task<GhnOrderDetailResponse> GetOrderDetailsFromGhnAsync(string orderCode);
    }

}