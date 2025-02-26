using Microsoft.AspNetCore.Mvc;
using OnlineShoppingSystem_Main.Models;
using Service.Interface;
using System.Diagnostics;
using System.Security.Claims;

namespace OnlineShoppingSystem_Main.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;

        public OrderController(IOrderService orderService, IUserService userService)
        {
            _orderService = orderService;
            _userService = userService;
        }

        private AspNetUser GetCurrentUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Debug.WriteLine($"[DEBUG] ClaimTypes.NameIdentifier: {userId}");

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
            Debug.WriteLine($"[DEBUG] User: {currentUser}");

            var model = await _orderService.CreateOrderConfirmationViewModelAsync(selectedCartItemIds, currentUser);

            return View("OrderConfirmation", model);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string fullName, string email, string mobile, string address, string paymentMethod, string selectedItems)
        {
            var cartItemIds = selectedItems.Split(",").Select(int.Parse).ToList();

            var order = await _orderService.CreateOrderAsync(fullName, email, mobile, address, paymentMethod, cartItemIds);

            return RedirectToAction("OrderSuccess", "Cart");
        }
    }
}