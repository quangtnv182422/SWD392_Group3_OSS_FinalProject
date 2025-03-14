﻿using Data.Models;
using Microsoft.AspNetCore.Identity;
using OnlineShoppingSystem_Main.Models;

namespace Service.Interface
{
    public interface IOrderService
    {
        Task<Order> GetOrderByIdAsync(string orderId);
        Task<OrderConfirmationViewModel> CreateOrderConfirmationViewModelAsync(List<int> selectedCartItemIds, IdentityUser currentUser);
        Task<Order> CreateOrderAsync(string fullName, string email, string mobile, string address, string paymentMethod, List<int> cartItemIds, float totalCost, int orderStatus);
        Task<Order> SaveOrderAsync(Order order);


        // Track Order List
        Task<List<Order>> GetOrdersByUserIdAsync(string userId);

        Task<bool> CancelOrderAsync(int orderId);


        // Track Order Detail
        Task<Order?> GetOrderDetailsAsync(int orderId);
        Task<bool> UpdateOrderAsync(Order order);
    }

}