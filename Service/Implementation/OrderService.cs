﻿using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingSystem_Main.Models;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation
{
	public class OrderService : IOrderService
	{
		private readonly IOrderRepository _orderRepository;
		private readonly IEmailSender _emailSender;

		public OrderService(IOrderRepository orderRepository, IEmailSender emailSender)
		{
			_orderRepository = orderRepository;
			_emailSender = emailSender;
		}

		public async Task<Order> GetOrderByIdAsync(string orderId)
		{
			return await _orderRepository.GetOrderByIdAsync(orderId);
		}

		public async Task<OrderConfirmationViewModel> CreateOrderConfirmationViewModelAsync(List<int> selectedCartItemIds, AspNetUser currentUser)
		{
			var selectedCartItems = await _orderRepository.GetCartItemsByIdsAsync(selectedCartItemIds);
			double totalPrice = selectedCartItems.Sum(ci => ci.Quantity * ci.Product.Price);

			return new OrderConfirmationViewModel
			{
				SelectedCartItems = selectedCartItems,
				SubTotal = totalPrice,
				FullName = currentUser?.UserName ?? "",
				Email = currentUser?.Email ?? "",
				Mobile = currentUser?.PhoneNumber ?? "",
				Address = ""
			};
		}

		public async Task<Order> CreateOrderAsync(string fullName,
												  string? customerId,
												  /*string staffId,*/
												  string email,
												  string mobile,
												  string address,
												  string paymentMethod,
												  List<int> cartItemIds,
												  float totalCost,
												  int orderStatus,
												  string? note)
		{
			var cartItems = await _orderRepository.GetCartItemsByIdsAsync(cartItemIds);

			if (cartItems == null || !cartItems.Any())
			{
				throw new InvalidOperationException("Cart items are empty or null.");
			}

			var order = new Order
			{
				OrderedAt = DateTime.Now,
				CustomerId = customerId,
				/*StaffId = staffId,*/
				PaymentMethod = paymentMethod,
				Address = address,
				OrderStatusId = orderStatus,
				OrderItems = cartItems.Select(ci => new OrderItem
				{
					ProductId = ci.ProductId,
					Quantity = ci.Quantity,
					PriceEachItem = ci.Product?.Price ?? 0
				}).ToList(),
				TotalCost = totalCost,
				FullName = fullName,
				Email = email,
				PhoneNumber = mobile,
				Note = note

			};

			await _orderRepository.CreateOrderAsync(order);
			//fix cứng tạm để test
			string link = "https://localhost:7268/Product/Index";

			await SendOrderConfirmEmail(email, fullName, address, mobile, note, cartItems.Select(ci => new OrderItem
			{
				ProductId = ci.ProductId,
				Quantity = ci.Quantity,
				PriceEachItem = ci.Product?.Price ?? 0,
				Product = ci.Product,
			}).ToList(), link, paymentMethod);

			return order;
		}

		//Gửi mail sau khi order:
		public async Task SendOrderConfirmEmail(string email, string fullName, string address, string phoneNumber, string? orderNotes, List<OrderItem> products, string returnLink, string paymentMethod)
		{
			var subject = "Order Confirmation - Sneaker Shoes";

			if (products == null || !products.Any())
			{
				products = new List<OrderItem>();
			}

			orderNotes = orderNotes ?? "No special notes";
			returnLink = returnLink ?? "#";
			paymentMethod = paymentMethod ?? "Not specified";

			var productRows = "";
			foreach (var product in products)
			{
				if (product?.Product != null)
				{
					productRows += $"\n" +
						$"    <tr class=\"product-item\">\n" +
						$"      <td><img src=\"{product.Product.ProductImages}\" alt=\"Đây là ảnh sách\"></td>\n" +
						$"      <td>{product.Product.ProductName}</td>\n" +
						$"      <td>{product.Product.Description}</td>\n" +
						$"      <td>{product.Product.Price:N2} VND</td>\n" +
						$"      <td>{(product.Product.Price * product.Quantity):N2} VND</td>\n" +
						$"      <td>{product.Quantity}</td>\n" +
						$"    </tr>";
				}
			}

			// Tạo nội dung email
			var message = "<!DOCTYPE html>\n"
						+ "<html>\n"
						+ "<head>\n"
						+ "    <style>\n"
						+ "        body { font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f4f4f4; }\n"
						+ "        .email-container { max-width: 800px; margin: 40px auto; background-color: #ffffff; padding: 20px; border: 1px solid #dddddd; border-radius: 10px; }\n"
						+ "        .header { text-align: center; padding: 10px 0; background-color: #e68d40; border-radius: 10px 10px 0 0; color: #ffffff; font-size: 24px; font-weight: bold; }\n"
						+ "        .header-icon img { height: 50px; margin: 20px 0; }\n"
						+ "        .content { padding: 20px; text-align: center; }\n"
						+ "        .content h1 { color: #333333; }\n"
						+ "        .content p { font-size: 16px; line-height: 1.5; color: #555555; }\n"
						+ "        .verify-button { display: inline-block; margin: 20px 0; padding: 15px 30px; font-size: 16px; color: #ffffff; background-color: #ce4b40; border-radius: 5px; text-decoration: none; }\n"
						+ "        .footer { text-align: center; padding: 20px; font-size: 14px; color: #aaaaaa; }\n"
						+ "        .footer a { color: #e68d40; text-decoration: none; }\n"
						+ "        .product-list { width: 100%; border-collapse: collapse; }\n"
						+ "        .product-list th, .product-list td { border: 1px solid #ddd; padding: 8px; text-align: left; }\n"
						+ "        .product-list th { background-color: #f2f2f2; }\n"
						+ "        .product-list img { max-width: 100px; height: auto; margin: auto; }\n"
						+ "        .product-item { background-color: #f9f9f9; }\n"
						+ "    </style>\n"
						+ "</head>\n"
						+ "<body>\n"
						+ "    <div class=\"email-container\">\n"
						+ "        <div class=\"header\">\n"
						+ "            BookShop<span>.</span>\n"
						+ "            <div class=\"header-icon\"><img src=\"https://cdn-icons-png.freepik.com/512/9840/9840606.png\" alt=\"Email Icon\"></div>\n"
						+ "        </div>\n"
						+ "        <div class=\"content\">\n"
						+ "            <h1>Đơn hàng đã được ghi nhận!</h1>\n"
						+ "            <p>Hi " + fullName + ",</p>\n"
						+ "            <p>Cảm ơn bạn đã chọn mua hàng tại BookShop, đơn hàng của bạn đã được ghi nhận .</p>\n"
						+ "            <h3>Thông tin đơn hàng</h3>\n"

						+ "<table border=\"1\">" +
								"<tr>" +
									"<td><strong>Họ và tên người mua:</strong></td>" +
									"<td>" + fullName + "</td>" +
								"</tr>" +
								"<tr>" +
									"<td><strong>Địa chỉ:</strong></td>" +
									"<td>" + address + "</td>" +
								"</tr>" +
								"<tr>" +
									"<td><strong>Số điện thoại:</strong></td>" +
									"<td>" + phoneNumber + "</td>" +
								"</tr>" +
								"<tr>" +
									"<td><strong>Ghi chú:</strong></td>" +
									"<td>" + orderNotes + "</td>" +
								"</tr>" +
								"<tr>" +
									"<td><strong>Phương thức thanh toán:</strong></td>" +
									"<td>" + paymentMethod + "</td>" +
								"</tr>" +
							"</table>"

						+ "            <h2>Sản phẩm đã chọn</h2>\n"
						+ "            <table class=\"product-list\">\n"
						+ "                <thead>\n"
						+ "                    <tr><th>Ảnh</th>" +
												"<th>Tên sách</th>" +
												"<th>Mô tả sách</th>" +
												"<th>Đơn giá</th>" +
												"<th>Tổng giá</th>" +
												"<th>Số lượng</th>" +
												"</tr>\n"
						+ "                </thead>\n"
						+ "                <tbody>" + productRows + "</tbody>\n"
						+ "            </table>\n"
						+ "            <a href=\"" + returnLink + "\" class=\"verify-button\">Tiếp tục mua sắm</a>\n"
						+ "        </div>\n"
						+ "        <div class=\"footer\">\n"
						+ "            <p>FPT University, Hoa Lac, Ha Noi</p>\n"
						+ "            <p><a href=\"#\">Privacy Policy</a> | <a href=\"#\">Contact Details</a></p>\n"
						+ "        </div>\n"
						+ "    </div>\n"
						+ "</body>\n"
						+ "</html>";

			await _emailSender.SendEmailAsync(email, subject, message);
		}


		public async Task<Order> SaveOrderAsync(Order order)
		{
			await _orderRepository.CreateOrderAsync(order);
			return order;
		}

		public async Task<bool> ConfirmOrderAsync(int orderId, int confirmStatus)
		{
			return await _orderRepository.ConfirmOrderAsync(orderId, confirmStatus);
		}

		// Track Order Detail
		public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
		{
			return await _orderRepository.GetOrdersByUserIdAsync(userId);
		}

		public async Task<bool> CancelOrderAsync(int orderId)
		{
			return await _orderRepository.CancelOrderAsync(orderId);
		}

		public async Task<Order> GetOrderDetailsAsync(int orderId)
		{
			return await _orderRepository.GetOrderDetailsAsync(orderId);
		}

		public async Task<bool> UpdateOrderAsync(Order order)
		{
			var existingOrder = await _orderRepository.GetOrderByIdAsync(order.OrderId.ToString());

			if (existingOrder == null)
			{
				return false;
			}

			existingOrder.Address = order.Address;
			existingOrder.PaymentMethod = order.PaymentMethod;
			existingOrder.OrderStatusId = order.OrderStatusId;
			existingOrder.Note = order.Note;

			await _orderRepository.SaveChangesAsync();
			return true;
		}
	}
}