using OnlineShoppingSystem_Main.Models;

namespace Service.Interface
{
    public interface IOrderService
    {
        Task<Order> GetOrderByIdAsync(string orderId);
        Task<OrderConfirmationViewModel> CreateOrderConfirmationViewModelAsync(List<int> selectedCartItemIds, AspNetUser currentUser);
        Task<Order> CreateOrderAsync(string fullName, string email, string mobile, string address, string paymentMethod, List<int> cartItemIds);
        Task<Order> SaveOrderAsync(Order order);
    }

}