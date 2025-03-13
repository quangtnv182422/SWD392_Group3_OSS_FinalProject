using Api.Payos.Interface;
using Api.vnPay.Interface;
using Azure;
using Data.Models.Vnpay;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Service.Interface;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OnlineShoppingSystem_Main.Controllers
{
	public class PaymentController : Controller
	{
		private readonly IVnPayProxy _vnPayService;
		private readonly IPayosProxy _payOsService;
		private readonly IOrderService _orderService;
		private readonly IConfiguration _configuration;

		public PaymentController(IVnPayProxy vnPayService,
								IOrderService orderService,
								IPayosProxy payOsService,
								IConfiguration configuration)
		{
			_vnPayService = vnPayService;
			_payOsService = payOsService;
			_orderService = orderService;
			_configuration = configuration;
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

					var paymentLinkRequest = new PaymentData(
											  orderCode: int.Parse(DateTimeOffset.Now.ToString("ffffff")),
											  amount: (int)totalCost,
											  description: "Thanh toan ma QR",
											  items: [new("Item test", 1, 2000)],
											  returnUrl: _configuration["PayOS:ReturnUrl"],
											  cancelUrl: _configuration["PayOS:CancelUrl"]);

					return RedirectToAction("CreatePaymentPayOS", "Payment", paymentLinkRequest);

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
		/// Trả về trang thanh toán thất bại
		/// </summary>
		public IActionResult PaymentFail()
		{
			return View();
		}
		/// <summary>
		/// Trả về trang thanh toán thành công
		/// </summary>
		public IActionResult PaymentSuccess()
		{
			return View();
		}

		/// <summary>
		/// Xử lý phản hồi từ VNPay sau khi thanh toán
		/// </summary>
		[HttpGet]
		public IActionResult PaymentCallbackVnpay()
		{
			var response = _vnPayService.PaymentExecute(Request.Query);
			if (response == null || response.VnPayResponseCode != "00")
			{
				TempData["Message"] = $"Lỗi thanh toán: {response.VnPayResponseCode}";
				return RedirectToAction("PaymentFail");
			}

			//Add order vào DB
			TempData["Message"] = $"Thanh toán thành công: {response.VnPayResponseCode}";
			return RedirectToAction("PaymentSuccess");
		}



		/// <summary>
		/// Xử lý thanh toán qua PayOS
		/// </summary>
		public async Task<IActionResult> CreatePaymentPayOS(PaymentData data)
		{
			var url = await _payOsService.CreatePayOSPaymentUrl(data);
			Response.Headers.Append("Location", url.checkoutUrl);
			return new StatusCodeResult(303);
		}

		/*/// <summary>
		/// Xử lý thông tin thanh toán trả về từ PayOS
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> PaymentCallbackPayOS()
		{
			if (response == null || response.VnPayResponseCode != "00")
			{
				TempData["Message"] = $"Lỗi thanh toán: {response.VnPayResponseCode}";
				return RedirectToAction("PaymentFail");
			}

			//Add order vào DB
			TempData["Message"] = $"Thanh toán thành công: {response.VnPayResponseCode}";
			return RedirectToAction("PaymentSuccess");
		}*/

	}
}
