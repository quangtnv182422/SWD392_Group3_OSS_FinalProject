using Data.Models;
using OnlineShoppingSystem_Main.Models;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Order> GetOrderByIdAsync(string orderId)
        {
            return await _orderRepository.GetOrderByIdAsync(orderId);
        }

        public async Task<OrderConfirmationViewModel> CreateOrderConfirmationViewModelAsync(List<int> selectedCartItemIds, IdentityUser currentUser)
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

        public async Task<Order> CreateOrderAsync(string fullName, string email, string mobile, string address, string paymentMethod, List<int> cartItemIds, float totalCost, int orderStatus)
        {
            var cartItems = await _orderRepository.GetCartItemsByIdsAsync(cartItemIds);

            var order = new Order
            {
                OrderedAt = DateTime.Now,
                PaymentMethod = paymentMethod,
                Address = address,
                OrderStatusId = orderStatus,
                OrderItems = cartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    PriceEachItem = ci.Product.Price
                }).ToList(),
                TotalCost = totalCost,
                FullName = fullName,
                Email = email,
                PhoneNumber = mobile
                
            };

            await _orderRepository.CreateOrderAsync(order);
            return order;
        }

        public async Task<Order> SaveOrderAsync(Order order)
        {
            await _orderRepository.CreateOrderAsync(order);
            return order;
        }

    }
}