using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using OnlineShoppingSystem_Main.Models;
using Newtonsoft.Json; 

namespace OnlineShoppingSystem_Main.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly ICartService _cartService;

        public OrderController(IOrderService orderService, IUserService userService, ICartService cartService)
        {
            _orderService = orderService;
            _userService = userService;
            _cartService = cartService;
        }

        private AspNetUser GetCurrentUser()
        {
            string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var user = _userService.GetCurrentUser(userId);
                if (user == null)
                {
                    throw new Exception("Không tìm thấy user với ID: " + userId);
                }
                else
                {
                    Debug.WriteLine($"[DEBUG] Đã tìm thấy user: {{ Id: {user.Id}, UserName: {user.UserName}, Email: {user.Email}, Phone: {user.PhoneNumber} }}");
                }
                return user;
            }
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOrder(List<string> selectedItems)
        {
            if (selectedItems == null || !selectedItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            List<int> selectedCartItemIds = selectedItems.Select(int.Parse).ToList();
            string userId = await _userService.GetUserIdAsync(HttpContext);
            var currentUser = GetCurrentUser();

            
            TempData["SelectedCartItemIds"] = JsonConvert.SerializeObject(selectedCartItemIds);

            var model = await _orderService.CreateOrderConfirmationViewModelAsync(selectedCartItemIds, currentUser);
            return View("OrderConfirmation", model);
        }

        [HttpPost]
        public async Task<IActionResult> BuyNow(int productId, int quantity = 1)
        {
            string userId = await _userService.GetUserIdAsync(HttpContext);
            var currentUser = GetCurrentUser();

            
            var cart = await _cartService.GetUserCartAsync(userId);
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    CartId = cart.CartId
                };
                cart.CartItems.Add(cartItem);
                await _cartService.UpdateCartAsync(cart);
            }
            else
            {
                cartItem.Quantity += quantity;
                await _cartService.UpdateCartItemAsync(cartItem);
            }

           
            List<int> selectedCartItemIds = new List<int>();
            if (TempData["SelectedCartItemIds"] != null)
            {
                selectedCartItemIds = JsonConvert.DeserializeObject<List<int>>(TempData["SelectedCartItemIds"].ToString());
            }

            
            if (!selectedCartItemIds.Contains(cartItem.CartItemId))
            {
                selectedCartItemIds.Add(cartItem.CartItemId);
            }

           
            TempData["SelectedCartItemIds"] = JsonConvert.SerializeObject(selectedCartItemIds);

            
            var model = await _orderService.CreateOrderConfirmationViewModelAsync(selectedCartItemIds, currentUser);
            return View("OrderConfirmation", model);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string fullName, string email, string mobile, string address, string paymentMethod, string selectedItems)
        {
            var cartItemIds = selectedItems.Split(",").Select(int.Parse).ToList();
            var order = await _orderService.CreateOrderAsync(fullName, email, mobile, address, paymentMethod, cartItemIds);

            TempData.Remove("SelectedCartItemIds");

            return RedirectToAction("OrderSuccess", "Cart");
        }
    }
}