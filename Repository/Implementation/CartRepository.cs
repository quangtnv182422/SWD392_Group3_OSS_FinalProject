using Microsoft.EntityFrameworkCore;
using OnlineShoppingSystem_Main.Data.Models;
using OnlineShoppingSystem_Main.Models;
using Repository.Interface;


namespace Repository.Implementation
{
    public class CartRepository : ICartRepository
    {
        private readonly Swd392OssContext _context;

        public CartRepository(Swd392OssContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == userId);
        }

        public async Task<Cart> CreateCartAsync(string userId)
        {
            var cart = new Cart { CustomerId = userId, CartItems = new List<CartItem>() };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<CartItem> GetCartItemByIdAsync(int cartItemId)
        {
            return await _context.CartItems.FindAsync(cartItemId);
        }

        public async Task UpdateCartItemAsync(CartItem cartItem)
        {
            _context.Update(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveCartItemAsync(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveCartAsync(Cart cart)
        {
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveCartItemsAsync(IEnumerable<CartItem> cartItems)
        {
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartAsync(Cart cart)
        {
            _context.Update(cart);
            await _context.SaveChangesAsync();
        }
    }

}