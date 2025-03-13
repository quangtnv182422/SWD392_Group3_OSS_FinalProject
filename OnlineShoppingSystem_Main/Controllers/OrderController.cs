using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineShoppingSystem_Main.Models;
using Service.Interface;
using System.Diagnostics;
using System.Security.Claims;
using Data.Models;
using Api.GHN.Interface;
using Data.Models.GHN; 

namespace OnlineShoppingSystem_Main.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly ICartService _cartService;
        private readonly IGhnProxy _ghnService;

        public OrderController(IOrderService orderService, IUserService userService, ICartService cartService, IGhnProxy ghnService)
        {
            _orderService = orderService;
            _userService = userService;
            _cartService = cartService;
            _ghnService = ghnService;
        }

        private async Task<IdentityUser> GetCurrentUserAsync()
        {
            string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userService.GetCurrentUserAsync(userId);
                if (user == null)
                {
                    throw new Exception("Không tìm thấy user với ID: " + userId);
                }
                Debug.WriteLine($"[DEBUG] Đã tìm thấy user: {{ Id: {user.Id}, UserName: {user.UserName}, Email: {user.Email}, Phone: {user.PhoneNumber} }}");
                return user;
            }
            return null;
        }


        [HttpGet]
        public async Task<IActionResult> GetProvinces()
        {
            var result = await _ghnService.GetProvincesAsync();
            return Content(result, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetDistricts(int provinceId)
        {
            var result = await _ghnService.GetDistrictsAsync(provinceId);
            return Content(result, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetWards(int districtId)
        {
            var result = await _ghnService.GetWardsAsync(districtId);
            return Content(result, "application/json");
        }

        [HttpPost]
        public async Task<IActionResult> CalculateShipping([FromBody] ShippingRequest request)
        {
            var result = await _ghnService.CalculateShippingFeeAsync(
                request.shopId, request.fromDistrictId, request.toDistrictId,
                request.weight, request.length, request.width, request.height,
                request.service_id, request.service_type_id);
            return Content(result, "application/json");
        }

        [HttpPost]
        public async Task<IActionResult> GetAvailableServices([FromBody] AvailableServicesRequest request)
        {
            var result = await _ghnService.GetAvailableServicesAsync(request.shopId, request.fromDistrictId, request.toDistrictId);
            return Content(result, "application/json");
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
            var currentUser = await GetCurrentUserAsync(); 

            TempData["SelectedCartItemIds"] = JsonConvert.SerializeObject(selectedCartItemIds);

            var model = await _orderService.CreateOrderConfirmationViewModelAsync(selectedCartItemIds, currentUser);
            return View("OrderConfirmation", model);
        }

        [HttpPost]
        public async Task<IActionResult> BuyNow(int productId, int quantity = 1)
        {
            string userId = await _userService.GetUserIdAsync(HttpContext); 
            var currentUser = await GetCurrentUserAsync(); 

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

            List<int> selectedCartItemIds = TempData["SelectedCartItemIds"] != null
                ? JsonConvert.DeserializeObject<List<int>>(TempData["SelectedCartItemIds"].ToString())
                : new List<int>();

            if (!selectedCartItemIds.Contains(cartItem.CartItemId))
            {
                selectedCartItemIds.Add(cartItem.CartItemId);
            }

            TempData["SelectedCartItemIds"] = JsonConvert.SerializeObject(selectedCartItemIds);

            var model = await _orderService.CreateOrderConfirmationViewModelAsync(selectedCartItemIds, currentUser);
            return View("OrderConfirmation", model);
        }

        /* [HttpPost]
         public async Task<IActionResult> PlaceOrder(string fullName, string email, string mobile, string address, string paymentMethod, string selectedItems)
         {
             var cartItemIds = selectedItems.Split(",").Select(int.Parse).ToList();
             //var order = await _orderService.CreateOrderAsync(fullName, email, mobile, address, paymentMethod, cartItemIds);

             TempData.Remove("SelectedCartItemIds");

             return RedirectToAction("OrderSuccess", "Cart");
         }*/


        // Track Order Detail
        [HttpGet]
        public async Task<IActionResult> OrderList(string searchOrderId, string paymentMethod, string status)
        {
            var currentUser = await GetCurrentUserAsync();

            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để xem danh sách đơn hàng.";
                return RedirectToAction("Index", "Home");
            }

            var orders = await _orderService.GetOrdersByUserIdAsync(currentUser.Id);

            // Filter
            if (!string.IsNullOrEmpty(searchOrderId))
            {
                orders = orders.Where(o => o.OrderId.ToString().Contains(searchOrderId)).ToList();
            }

            if (!string.IsNullOrEmpty(paymentMethod))
            {
                orders = orders.Where(o => o.PaymentMethod == paymentMethod).ToList();
            }

            if (!string.IsNullOrEmpty(status))
            {
                orders = orders.Where(o => o.OrderStatus.StatusName == status).ToList();
            }

            ViewBag.SearchOrderId = searchOrderId;
            ViewBag.PaymentMethod = paymentMethod;
            ViewBag.Status = status;

            return View("OrderList", orders);
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            bool isCancelled = await _orderService.CancelOrderAsync(orderId);
            if (!isCancelled)
            {
                TempData["ErrorMessage"] = "Không thể hủy đơn hàng hoặc đơn hàng không tồn tại.";
            }
            else
            {
                TempData["SuccessMessage"] = "Đơn hàng đã được hủy thành công.";
            }
            return RedirectToAction("OrderList");
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetails(int orderId)
        {
            var order = await _orderService.GetOrderDetailsAsync(orderId);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("OrderList");
            }

            return View("OrderDetails", order);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderDetails(Order updatedOrder)
        {
            var existingOrder = await _orderService.GetOrderByIdAsync(updatedOrder.OrderId.ToString());

            if (existingOrder == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("OrderList");
            }

            try
            {
                existingOrder.FullName = updatedOrder.FullName;
                existingOrder.PhoneNumber = updatedOrder.PhoneNumber;
                existingOrder.Email = updatedOrder.Email;
                existingOrder.Address = updatedOrder.Address;
                existingOrder.OrderStatusId = updatedOrder.OrderStatusId;
                existingOrder.Note = updatedOrder.Note;

                await _orderService.UpdateOrderAsync(existingOrder);

                TempData["SuccessMessage"] = "Cập nhật đơn hàng thành công.";
                return RedirectToAction("OrderDetails", new { orderId = updatedOrder.OrderId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi cập nhật đơn hàng: " + ex.Message;
                return RedirectToAction("OrderDetails", new { orderId = updatedOrder.OrderId });
            }
        }
    }
}