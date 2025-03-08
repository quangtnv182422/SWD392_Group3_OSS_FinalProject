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
    }

}