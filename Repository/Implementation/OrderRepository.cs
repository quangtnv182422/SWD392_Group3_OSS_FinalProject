using Data.Models;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingSystem_Main.Models;
using Repository.Interface;

namespace Repository.Implementation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly Swd392OssContext _context;

        public OrderRepository(Swd392OssContext context)
        {
            _context = context;
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


        // Lấy danh sách đơn hàng theo UserId
        public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == userId)
                .Include(o => o.OrderStatus)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }

        // Hủy đơn hàng (cập nhật trạng thái thành 'Cancelled')
        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return false;
            }

            // Cập nhật trạng thái đơn hàng thành 'Cancelled'
            order.OrderStatusId = 5; // StatusId = 5 tương ứng với 'Cancelled'
            await _context.SaveChangesAsync();
            return true;
        }
    }
}