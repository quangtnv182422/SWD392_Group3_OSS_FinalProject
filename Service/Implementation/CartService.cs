using Data.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OnlineShoppingSystem_Main.Models;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository, IHttpContextAccessor httpContextAccessor)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Cart> GetUserCartAsync(string userId)
        {
            Cart cart;
            if (!string.IsNullOrEmpty(userId))
            {
                cart = await _cartRepository.GetCartByUserIdAsync(userId);
                if (cart == null)
                {
                    cart = await _cartRepository.CreateCartAsync(userId);
                }
                Console.WriteLine($"[DEBUG] Retrieved Cart from DB - UserId: {userId}, CartItems: {string.Join(", ", cart.CartItems.Select(ci => $"CartItemId: {ci.CartItemId}, ProductId: {ci.ProductId}"))}");
            }
            else
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                var cartJson = session.GetString("Cart");
                cart = cartJson != null ? JsonConvert.DeserializeObject<Cart>(cartJson) : new Cart
                {
                    CartItems = new List<CartItem>()
                };
                Console.WriteLine($"[DEBUG] Retrieved Cart from Session - CartItems: {string.Join(", ", cart.CartItems.Select(ci => $"CartItemId: {ci.CartItemId}, ProductId: {ci.ProductId}"))}");
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
            Console.WriteLine($"[DEBUG] Attempting to remove CartItemId: {cartItemId}");

            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);
            if (cartItem != null)
            {
                Console.WriteLine($"[DEBUG] Found CartItem in DB: {cartItemId}");
                await _cartRepository.RemoveCartItemAsync(cartItem);
                return true;
            }

            var cart = await GetUserCartAsync(null);
            Console.WriteLine($"[DEBUG] CartItems in Session: {cart.CartItems.Count}");
            var sessionCartItem = cart.CartItems.FirstOrDefault(ci => ci.CartItemId == cartItemId);
            if (sessionCartItem != null)
            {
                Console.WriteLine($"[DEBUG] Found CartItem in Session: {cartItemId}");
                cart.CartItems.Remove(sessionCartItem);

                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                _httpContextAccessor.HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart, settings));
                Console.WriteLine("[DEBUG] Session updated after removal");
                return true;
            }

            Console.WriteLine($"[DEBUG] CartItemId {cartItemId} not found");
            return false;
        }

        public async Task<Order> PlaceOrderAsync(string userId)
        {
            var cart = await GetUserCartAsync(userId);
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
            if (!string.IsNullOrEmpty(userId))
            {
                await _cartRepository.RemoveCartAsync(cart);
            }
            else
            {
                _httpContextAccessor.HttpContext.Session.Remove("Cart");
            }
            return order;
        }

        public async Task<Order> PlaceSelectedOrderAsync(string userId, List<int> selectedCartItemIds)
        {
            var cart = await GetUserCartAsync(userId);
            if (cart == null || !cart.CartItems.Any())
            {
                Console.WriteLine("[DEBUG] Cart is null or empty");
                return null;
            }

            var selectedCartItems = cart.CartItems
                .Where(ci => selectedCartItemIds.Contains(ci.CartItemId))
                .ToList();

            if (!selectedCartItems.Any())
            {
                Console.WriteLine("[DEBUG] No selected cart items found");
                return null;
            }

            foreach (var item in selectedCartItems)
            {
                Console.WriteLine($"[DEBUG] Selected CartItem - CartItemId: {item.CartItemId}, ProductId: {item.ProductId}, Quantity: {item.Quantity}");
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

            if (!string.IsNullOrEmpty(userId))
            {
                await _cartRepository.RemoveCartItemsAsync(selectedCartItems);
                if (!cart.CartItems.Except(selectedCartItems).Any())
                {
                    await _cartRepository.RemoveCartAsync(cart);
                }
            }
            else
            {
                var itemsToRemove = cart.CartItems.Where(ci => selectedCartItemIds.Contains(ci.CartItemId)).ToList();
                foreach (var item in itemsToRemove)
                {
                    cart.CartItems.Remove(item);
                }
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.Indented
                };
                _httpContextAccessor.HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart, settings));
                Console.WriteLine($"[DEBUG] Session updated, remaining CartItems: {cart.CartItems.Count}");
            }

            Console.WriteLine($"[DEBUG] Order created with OrderItems: {string.Join(", ", order.OrderItems.Select(oi => $"ProductId: {oi.ProductId}, Quantity: {oi.Quantity}"))}");
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
        public async Task<bool> AddProductToCartAsync(string userId, int productId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                return await _cartRepository.AddProductToCartAsync(userId, productId);
            }
            else
            {
                var cart = await GetUserCartAsync(null);
                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
                if (cartItem == null)
                {
                    var product = await _cartRepository.GetProductByIdAsync(productId);
                    if (product == null)
                    {
                        Console.WriteLine($"[DEBUG] Product with ProductId: {productId} not found");
                        return false;
                    }
                    cartItem = new CartItem
                    {
                        CartItemId = cart.CartItems.Any() ? cart.CartItems.Max(ci => ci.CartItemId) + 1 : 1,
                        ProductId = productId,
                        Quantity = 1,
                        Product = product
                    };
                    Console.WriteLine($"[DEBUG] Added CartItem - CartItemId: {cartItem.CartItemId}, ProductId: {cartItem.ProductId}");
                    cart.CartItems.Add(cartItem);
                }
                else
                {
                    cartItem.Quantity++;
                    Console.WriteLine($"[DEBUG] Updated CartItem - CartItemId: {cartItem.CartItemId}, ProductId: {cartItem.ProductId}");
                }

                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                _httpContextAccessor.HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart, settings));
                Console.WriteLine($"[DEBUG] Cart saved to Session - CartItems: {string.Join(", ", cart.CartItems.Select(ci => $"CartItemId: {ci.CartItemId}, ProductId: {ci.ProductId}"))}");
                return true;
            }
        }
    }

}