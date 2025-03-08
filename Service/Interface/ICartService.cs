using Data.Models;
using OnlineShoppingSystem_Main.Models;


namespace Service.Interface
{
    public interface ICartService
    {
        Task<Cart> GetUserCartAsync(string userId);
        Task<bool> UpdateCartItemQuantityAsync(int cartItemId, int quantity);
        Task<bool> RemoveCartItemAsync(int cartItemId);
        Task<Order> PlaceOrderAsync(string userId);
        Task<Order> PlaceSelectedOrderAsync(string userId, List<int> selectedCartItemIds);

        Task UpdateCartAsync(Cart cart); 
        Task UpdateCartItemAsync(CartItem cartItem); 
    }

}