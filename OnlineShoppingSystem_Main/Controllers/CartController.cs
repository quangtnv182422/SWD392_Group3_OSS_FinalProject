using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using Newtonsoft.Json;
using Data.Models;

namespace OnlineShoppingSystem_Main.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;

        public CartController(ICartService cartService, IOrderService orderService, IUserService userService)
        {
            _cartService = cartService;
            _orderService = orderService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            string userId = await _userService.GetUserIdAsync(HttpContext);
            var cart = await _cartService.GetUserCartAsync(userId);
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            Console.WriteLine($"Received cartItemId: {cartItemId}, quantity: {quantity}");
            bool result = await _cartService.UpdateCartItemQuantityAsync(cartItemId, quantity);
            if (result)
            {
                TempData["Message"] = quantity <= 0
                    ? "Đã xóa sản phẩm khỏi giỏ hàng."
                    : "Cập nhật số lượng thành công.";
            }
            else
            {
                Console.WriteLine("Cart item not found!");
                TempData["Error"] = "Không tìm thấy sản phẩm trong giỏ hàng.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            bool result = await _cartService.RemoveCartItemAsync(cartItemId);
            if (result)
            {
                TempData["Message"] = "Xóa sản phẩm khỏi giỏ hàng thành công.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> PlaceSelectedOrder(List<string> selectedItems)
        {
            if (selectedItems == null || !selectedItems.Any())
            {
                TempData["Error"] = "Bạn chưa chọn sản phẩm nào để đặt hàng!";
                return RedirectToAction("Index");
            }

            string userId = await _userService.GetUserIdAsync(HttpContext);
            var selectedCartItemIds = selectedItems
                .Select(id => int.TryParse(id, out int parsedId) ? parsedId : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();

            var cart = await _cartService.GetUserCartAsync(userId);
            var validCartItemIds = new List<int>();
            var outOfStockItems = new List<string>();

            // Kiểm tra tồn kho
            foreach (var cartItemId in selectedCartItemIds)
            {
                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.CartItemId == cartItemId);
                if (cartItem != null)
                {
                    if (cartItem.Product.Quantity >= cartItem.Quantity) // Còn hàng
                    {
                        validCartItemIds.Add(cartItemId);
                    }
                    else // Hết hàng
                    {
                        outOfStockItems.Add(cartItem.Product.ProductName);
                    }
                }
            }

            if (outOfStockItems.Any())
            {
                TempData["Error"] = $"Sản phẩm {string.Join(", ", outOfStockItems)} đã hết hàng. Vui lòng kiểm tra lại!";
                TempData["SelectedCartItemIds"] = JsonConvert.SerializeObject(selectedCartItemIds); // Giữ lại lựa chọn
                return RedirectToAction("Index");
            }

            if (!validCartItemIds.Any())
            {
                TempData["Error"] = "Không có sản phẩm hợp lệ để đặt hàng!";
                return RedirectToAction("Index");
            }

            TempData["SelectedCartItemIds"] = JsonConvert.SerializeObject(validCartItemIds);
            var order = await _cartService.PlaceSelectedOrderAsync(userId, validCartItemIds);

            if (order == null)
            {
                TempData["Error"] = "Không có sản phẩm hợp lệ để đặt hàng!";
                return RedirectToAction("Index");
            }

            order = await _orderService.SaveOrderAsync(order);
            TempData.Remove("SelectedCartItemIds");

            TempData["Message"] = "Đặt hàng thành công!";
            return RedirectToAction("OrderSuccess", new { orderId = order.OrderId });
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            string userId = await _userService.GetUserIdAsync(HttpContext);
            var order = await _cartService.PlaceOrderAsync(userId);

            if (order == null)
            {
                TempData["Error"] = "Giỏ hàng trống. Không thể đặt hàng!";
                return RedirectToAction("Index");
            }

            order = await _orderService.SaveOrderAsync(order);

            TempData["Message"] = "Đặt hàng thành công!";
            return RedirectToAction("OrderSuccess", new { orderId = order.OrderId });
        }

        public async Task<IActionResult> OrderSuccess(string orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng!";
                return RedirectToAction("Index");
            }
            return View(order);
        }
    }
}