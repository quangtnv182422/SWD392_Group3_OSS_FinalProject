using Api.vnPay.Interface;
using Data.Models.Vnpay;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OnlineShoppingSystem_Main.Controllers
{
	public class PaymentController : Controller
	{
		private readonly IVnPayService _vnPayService;
		private readonly IOrderService _orderService; // Thêm OrderService để xử lý COD

		public PaymentController(IVnPayService vnPayService, IOrderService orderService)
		{
			_vnPayService = vnPayService;
			_orderService = orderService;
		}

		/// <summary>
		/// Xử lý thanh toán từ form checkout
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> ProcessPayment(string fullName, string email, string mobile, string address, string paymentMethod, string selectedItems, decimal totalCost)
		{
			var cartItemIds = selectedItems.Split(",").Select(int.Parse).ToList();


			switch (paymentMethod)
			{
				case "COD":
					var order = await _orderService.CreateOrderAsync(fullName, email, mobile, address, paymentMethod, cartItemIds, (float)totalCost, 2); // 2 là confirm order luôn ko cần pending
					return RedirectToAction("OrderSuccess", "Cart");

				case "vnPay":

					var paymentModel = new PaymentInformationModel
					{
						OrderType = "other",
						Amount = (double)totalCost,
						OrderDescription = "Thanh toán bằng vnPay tại sneaker Shoes",
						Name = fullName
					};
					return RedirectToAction("CreatePaymentUrlVnpay", "Payment", paymentModel);

				case "PayOS":
					return RedirectToAction("CreatePaymentPayOS", "Payment", new { fullName, email, mobile, address, selectedItems, totalCost });

				default:
					return RedirectToAction("Checkout", "Cart");
			}
		}


		/// <summary>
		/// Tạo URL thanh toán VNPay và chuyển hướng đến VNPay
		/// </summary>
		public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
		{
			var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
			return Redirect(url);
		}

		/// <summary>
		/// Xử lý phản hồi từ VNPay sau khi thanh toán
		/// </summary>
		[HttpGet]
		public IActionResult PaymentCallbackVnpay()
		{
			var response = _vnPayService.PaymentExecute(Request.Query);
			return Json(response);
		}

		

		/// <summary>
		/// Xử lý thanh toán qua PayOS
		/// </summary>
		public IActionResult CreatePaymentPayOS(string fullName, string email, string mobile, string address, string selectedItems, decimal totalCost)
		{
			// TODO: Thêm logic gọi API PayOS để tạo QR Code thanh toán
			return Content("Tính năng thanh toán PayOS đang được triển khai...");
		}

		public IActionResult Index()
		{
			return View();
		}
	}
}
