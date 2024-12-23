using Plantshop.ViewModels;
using PlantShop.Models;

namespace Plantshop.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string userId, CheckoutViewModel model);
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<List<Order>> GetUserOrdersAsync(string userId);
    }
}
