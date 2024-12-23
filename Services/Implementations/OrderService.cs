using Microsoft.EntityFrameworkCore;
using Plantshop.Data;
using Plantshop.Services.Interfaces;
using Plantshop.ViewModels;
using PlantShop.Models;

namespace Plantshop.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;

        public OrderService(ApplicationDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        public async Task<Order> CreateOrderAsync(string userId, CheckoutViewModel model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cartItems = await _cartService.GetCartItemsAsync(userId);
                if(!cartItems.Any())
                    throw new InvalidOperationException("Кошик порожній");

                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = await _cartService.GetCartTotalAsync(userId),
                    Status = "Нове",
                    ShippingAddress = model.ShippingAddress,
                    PaymentStatus = "Очікує оплати",
                    PaymentMethod = model.PaymentMethod,
                    DeliveryMethod = model.DeliveryMethod,
                    OrderItems = cartItems.Select(ci => new OrderItem
                    {
                        PlantId = ci.PlantId,
                        Quantity = ci.Quantity,
                        PriceAtTime = ci.Plant.CurrentPrice
                    }).ToList()
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                await _cartService.ClearCartAsync(userId);
                await transaction.CommitAsync();

                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Plant)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<List<Order>> GetUserOrdersAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Plant)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
    }
}
