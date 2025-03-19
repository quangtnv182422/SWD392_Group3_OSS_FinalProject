using Api.GHN.Implementation;
using Api.GHN.Interface;
using Data.Models;
using Data.Models.GHN;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingSystem_Main.Models;
using Repository.Interface;

namespace Repository.Implementation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly Swd392OssContext _context;
        private readonly IGhnProxy _ghnProxy;

        public OrderRepository(Swd392OssContext context, IGhnProxy ghnProxy)
        {
            _context = context;
            _ghnProxy = ghnProxy;
        }

        public async Task<Order> GetOrderByIdAsync(string orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId.ToString() == orderId);
        }

        public async Task<List<CartItem>> GetCartItemsByIdsAsync(List<int> cartItemIds)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .Include(a => a.Product.ProductImages)
                .Where(ci => cartItemIds.Contains(ci.CartItemId))
                .ToListAsync();
        }

        public async Task CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

		public async Task<bool> ConfirmOrderAsync(int orderId, int confirmStatus)
		{
			var order = await _context.Orders.FindAsync(orderId);
			if (order == null)
			{
				return false;
			}

			order.OrderStatusId = confirmStatus; 
			await _context.SaveChangesAsync();
			return true;
		}

		// Track Order Detail
		public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == userId)
                .Include(o => o.OrderStatus)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }

        public async Task<GhnOrderDetailResponse> GetOrderDetailsFromGhnAsync(string orderCode)
        {
            return await _ghnProxy.GetOrderDetailsFromGhnAsync(orderCode);
        }

        //public async Task<bool> CancelOrderAsync(int orderId)
        //{
        //    var order = await _context.Orders.FindAsync(orderId);
        //    if (order == null)
        //    {
        //        return false;
        //    }

        //    order.OrderStatusId = 5; // StatusId = 5 corresponds to 'Cancelled'
        //    await _context.SaveChangesAsync();
        //    return true;
        //}

        public async Task<bool> CancelOrderAsync(string orderCode)
        {
            return await _ghnProxy.CancelOrderOnGhnAsync(orderCode);
        }

        public async Task<Order> GetOrderDetailsAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderStatus)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            var existingOrder = await _context.Orders.FindAsync(order.OrderId);
            if (existingOrder == null)
            {
                return false; 
            }

            existingOrder.PaymentMethod = order.PaymentMethod;
            existingOrder.Note = order.Note;
            existingOrder.Address = order.Address;
            existingOrder.OrderStatusId = order.OrderStatusId;

            _context.Orders.Update(existingOrder);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}