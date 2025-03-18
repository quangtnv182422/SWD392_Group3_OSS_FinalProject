using Data.Models;
using OnlineShoppingSystem_Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface ICartRepository
    {
        Task<Cart> GetCartByUserIdAsync(string userId);
        Task<Cart> CreateCartAsync(string userId);
        Task<CartItem> GetCartItemByIdAsync(int cartItemId);
        Task UpdateCartItemAsync(CartItem cartItem);
        Task RemoveCartItemAsync(CartItem cartItem);
        Task RemoveCartAsync(Cart cart);
        Task RemoveCartItemsAsync(IEnumerable<CartItem> cartItems);
        Task SaveChangesAsync();

        Task UpdateCartAsync(Cart cart);

        //add to cart cua duc anh
        Task<bool> AddProductToCartAsync(string userId, int productId);

    }

}