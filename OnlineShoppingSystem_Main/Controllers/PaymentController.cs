using Api.Payos.Interface;
using Api.vnPay.Interface;
using Azure;
using Data.Models.Vnpay;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using Service.Interface;
using Data.Models.PayOS;
using Api.GHN.Implementation;
using Data.Models.GHN;
using Api.GHN.Interface;
namespace OnlineShoppingSystem_Main.Controllers
{
	//	9704198526191432198
	// NGUYEN VAN A
	// 07/15
	public class PaymentController : Controller
	{
		private readonly IVnPayProxy _vnPayService;
		private readonly IPayosProxy _payOsService;
		private readonly IOrderService _orderService;
		private readonly IConfiguration _configuration;
		private readonly IUserService _userService;
		private readonly IGhnProxy _ghnService;

		public PaymentController(IVnPayProxy vnPayService,
								IOrderService orderService,
								IPayosProxy payOsService,
								IConfiguration configuration,
								IUserService userService,
								IGhnProxy ghnService
							  )
		{
			_vnPayService = vnPayService;
			_payOsService = payOsService;
			_orderService = orderService;
			_configuration = configuration;
			_userService = userService;
			_ghnService = ghnService;
		}

		/// <summary>
		/// Xử lý thanh toán từ form checkout
		/// </summary>
		[HttpPost]
		public async Task<IActionResult> ProcessPayment(string fullName,
														string email,
														string mobile,
														string address,
														string paymentMethod,
														string selectedItems,
														decimal totalCost,
														string deliveryNotes,
														string SelectedProvinceName,
														string SelectedDistrictName,
														string SelectedWardname,
														string SelectedProvinceId,
														int SelectedDistrictId,
														string SelectedWardId)
		{
			//Lấy từng sản phẩm trong giỏ hàng 
			var cartItemIds = selectedItems.Split(",").Select(int.Parse).ToList();
			//Lấy full địa chỉ
			string fullAddress = string.Join(", ", new[] { address, SelectedWardname, SelectedDistrictName, SelectedProvinceName }
									  .Where(part => !string.IsNullOrEmpty(part)));
			//Lấy Id user nếu có đăng nhập
			var userId = await _userService.GetCurrentUserIdAsync();
			//Tạo order lưu vào DB với trạng thái pending
			var order = await _orderService.CreateOrderAsync(fullName, userId, email, mobile,
												fullAddress, paymentMethod, cartItemIds,
												(float)totalCost, 1, deliveryNotes); // 1 là pending confirm dành cho COD
			switch (paymentMethod)
			{
				case "COD":
					var shippingOrder = new ShippingOrder
					{
						payment_type_id = 2,
						note = deliveryNotes,
						required_note = "KHONGCHOXEMHANG",
						to_name = fullName,
						to_phone = mobile,
						to_address = fullAddress,
						to_ward_code = SelectedWardId,
						to_district_id = SelectedDistrictId,
						cod_amount = 20000,//max tối đa COD của GHN là 300k
						weight = 200,
						length = 20,
						width = 15,
						height = 5,
						service_type_id = 2,
						items = new List<ShippingOrder.ItemOrderGHN>
							{
								new ShippingOrder.ItemOrderGHN
								{
									name = "Sách đọc",
									quantity = 1,
									weight = 1200,
									category = new ShippingOrder.ItemOrderGHN.CategoryGHN { level1 = "Sách" }
								}
							}

					};

					var shippingResponse = await _ghnService.SendShippingOrderAsync(shippingOrder);
					if (shippingResponse.Contains("Error"))
					{
						return BadRequest("Failed to create shipping order: " + shippingResponse);
					}
					return RedirectToAction("PaymentSuccess");

				case "vnPay":

					var paymentModel = new PaymentInformationModel
					{
						OrderType = "other",
						Amount = (double)totalCost,
						OrderDescription = "Thanh toán bằng vnPay tại sneaker Shoes",
						Name = fullName,
						OrderId = order.OrderId.ToString(),
					};

					return RedirectToAction("CreatePaymentUrlVnpay", "Payment", paymentModel);

				case "PayOS":

					var paymentLinkRequest = new PaymentData(
											  //orderCode: int.Parse(DateTimeOffset.Now.ToString("ffffff")),
											  orderCode: order.OrderId,
											  amount: (int)totalCost,
											  description: "Thanh toan ma QR",
											  items: [new("Item test", 1, 2000)],
											  returnUrl: _configuration["PayOS:PayOSReturnUrl"],
											  cancelUrl: _configuration["PayOS:PayOSReturnUrl"]);

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
		public async Task<IActionResult> PaymentCallbackVnpay()
		{
			var response = _vnPayService.PaymentExecute(Request.Query);
			if (response == null || response.VnPayResponseCode != "00")
			{
				TempData["Message"] = $"Lỗi thanh toán: {response.VnPayResponseCode}";
				return RedirectToAction("PaymentFail");
			}

			TempData["Message"] = $"Thanh toán thành công: {response.VnPayResponseCode}";
			//Đổi order status sang 2
			if (int.TryParse(response.OrderId, out int orderId))
			{
				var updateOrder = await _orderService.GetOrderByIdAsync(response.OrderId);
				if (updateOrder != null)
				{
					await _orderService.ConfirmOrderAsync(updateOrder.OrderId, 2); // 2 là status đã xác nhận

				}
			}
			else
			{
				TempData["Message"] = "Lỗi xác nhận đơn hàng: Không thể chuyển đổi OrderId.";
			}
			return RedirectToAction("PaymentSuccess");
		}



		/// <summary>
		/// Xử lý thanh toán qua PayOS
		/// </summary>
		public async Task<IActionResult> CreatePaymentPayOS(PaymentData data)
		{
			var url = await _payOsService.CreatePayOSPaymentUrl(data);
			return Redirect(url.checkoutUrl);
		}


		/// <summary>
		/// Xử lý thông tin thanh toán trả về từ PayOS
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> PaymentCallbackPayOS()
		{
			var response = _payOsService.ProcessReturnUrl(Request.Query);

			if (response == null || response.Code != "00")
			{
				return RedirectToAction("PaymentFail");
			}
			else if (response.Status == "PAID")
			{
				//Đổi order status thành 2
				if (int.TryParse(response.OrderCode, out int orderId))
				{
					var updateOrder = await _orderService.GetOrderByIdAsync(response.OrderCode);
					if (updateOrder != null)
					{
						await _orderService.ConfirmOrderAsync(updateOrder.OrderId, 2); // 2 là status đã xác nhận

					}
				}
				else
				{
					TempData["Message"] = "Lỗi xác nhận đơn hàng: Không thể chuyển đổi OrderId.";
				}
			}
			return RedirectToAction("PaymentSuccess");
		}

	}
}
