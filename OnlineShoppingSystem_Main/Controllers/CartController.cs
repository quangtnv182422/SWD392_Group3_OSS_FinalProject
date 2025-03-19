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
        private readonly IProductService _productService;

        public CartController(ICartService cartService, IOrderService orderService, IUserService userService, IProductService productService)
        {
            _cartService = cartService;
            _orderService = orderService;
            _userService = userService;
            _productService = productService;
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
            bool result = await _cartService.UpdateCartItemQuantityAsync(cartItemId, quantity);
            if (result)
            {
                TempData["Message"] = quantity <= 0
                    ? "Đã xóa sản phẩm khỏi giỏ hàng."
                    : "Cập nhật số lượng thành công.";
            }
            else
            {
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
            else
            {
                TempData["Error"] = "Không thể xóa sản phẩm khỏi giỏ hàng.";
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
            Console.WriteLine($"[DEBUG] SelectedCartItemIds: {string.Join(",", selectedCartItemIds)}");
            Console.WriteLine($"[DEBUG] CartItems in cart: {string.Join(",", cart.CartItems.Select(ci => $"{ci.CartItemId} (ProductId: {ci.ProductId})"))}");

            var validCartItemIds = new List<int>();
            var outOfStockItems = new List<string>();

            foreach (var cartItemId in selectedCartItemIds)
            {
                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.CartItemId == cartItemId);
                if (cartItem != null)
                {
                    if (cartItem.Product.Quantity >= cartItem.Quantity)
                    {
                        validCartItemIds.Add(cartItemId);
                    }
                    else
                    {
                        outOfStockItems.Add(cartItem.Product.ProductName);
                    }
                }
                else
                {
                    Console.WriteLine($"[DEBUG] CartItemId {cartItemId} not found in cart");
                }
            }

            if (outOfStockItems.Any())
            {
                TempData["Error"] = $"Sản phẩm {string.Join(", ", outOfStockItems)} đã hết hàng. Vui lòng kiểm tra lại!";
                TempData["SelectedCartItemIds"] = JsonConvert.SerializeObject(selectedCartItemIds);
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
            return RedirectToAction("ConfirmOrder", new { orderId = order.OrderId });
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

        [HttpGet]
        public async Task<IActionResult> AddToCart(int id)
        {
            string userId = await _userService.GetUserIdAsync(HttpContext);
            var product = _productService.GetProductById(id);

            if (product == null)
            {
                TempData["Error"] = "Sản phẩm không tồn tại!";
                return RedirectToAction("Index", "Home");
            }

            bool result = await _cartService.AddProductToCartAsync(userId, id);

            if (result)
            {
                TempData["Message"] = "Sản phẩm đã được thêm vào giỏ hàng!";
            }
            else
            {
                TempData["Error"] = "Có lỗi khi thêm sản phẩm vào giỏ hàng!";
            }

            return RedirectToAction("Index", "Cart");
        }
    }
}