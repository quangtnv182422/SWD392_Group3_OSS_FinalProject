using Data.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using OnlineShoppingSystem_Main.Models;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<Cart> GetUserCartAsync(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                cart = await _cartRepository.CreateCartAsync(userId);
            }

            return cart;
        }

        public async Task<bool> UpdateCartItemQuantityAsync(int cartItemId, int quantity)
        {
            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);
            if (cartItem == null)
            {
                return false;
            }

            if (quantity <= 0)
            {
                await _cartRepository.RemoveCartItemAsync(cartItem);
            }
            else
            {
                cartItem.Quantity = quantity;
                await _cartRepository.UpdateCartItemAsync(cartItem);
            }

            return true;
        }

        public async Task<bool> RemoveCartItemAsync(int cartItemId)
        {
            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);
            if (cartItem == null)
            {
                return false;
            }

            await _cartRepository.RemoveCartItemAsync(cartItem);
            return true;
        }

        public async Task<Order> PlaceOrderAsync(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
            {
                return null;
            }

            var order = new Order
            {
                OrderedAt = DateTime.Now,
                CustomerId = userId,
                OrderStatusId = 1,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    PriceEachItem = ci.Product.Price
                }).ToList()
            };

            await _cartRepository.RemoveCartAsync(cart);

            return order;
        }

        public async Task<Order> PlaceSelectedOrderAsync(string userId, List<int> selectedCartItemIds)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
            {
                return null;
            }

            var selectedCartItems = cart.CartItems
                .Where(ci => selectedCartItemIds.Contains(ci.CartItemId))
                .ToList();

            if (!selectedCartItems.Any())
            {
                return null;
            }

            var order = new Order
            {
                OrderedAt = DateTime.Now,
                CustomerId = userId,
                OrderStatusId = 1,
                OrderItems = selectedCartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    PriceEachItem = ci.Product.Price
                }).ToList()
            };

            // Remove selected cart items
            await _cartRepository.RemoveCartItemsAsync(selectedCartItems);

            // If cart is empty after removal, remove the cart
            if (!cart.CartItems.Except(selectedCartItems).Any())
            {
                await _cartRepository.RemoveCartAsync(cart);
            }

            return order;
        }

        public async Task UpdateCartAsync(Cart cart)
        {
            await _cartRepository.UpdateCartAsync(cart);
        }

        public async Task UpdateCartItemAsync(CartItem cartItem)
        {
            await _cartRepository.UpdateCartItemAsync(cartItem);
        }
    }

}