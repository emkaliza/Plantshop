using PlantShop.Models;

namespace Plantshop.Services.Interfaces
{
    public interface ICartService
    {
        Task<Cart> GetOrCreateCartAsync(string userId);
        Task AddToCartAsync(int plantId, string userId, int quantity);
        Task<int> GetCartItemsCountAsync(string userId);
        Task ClearCartAsync(string userId);
        Task<List<CartItem>> GetCartItemsAsync(string userId);
        Task UpdateCartItemQuantityAsync(int cartItemId, int quantity);
        Task RemoveCartItemAsync(int cartItemId);
        Task<decimal> GetCartTotalAsync(string userId);
    }
}
