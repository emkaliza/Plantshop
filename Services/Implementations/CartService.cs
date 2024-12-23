using Microsoft.EntityFrameworkCore;
using Plantshop.Data;
using Plantshop.Services.Interfaces;
using PlantShop.Models;

namespace Plantshop.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPlantService _plantService;

        public CartService(ApplicationDbContext context, IPlantService plantService)
        {
            _context = context;
            _plantService = plantService;
        }

        public async Task<Cart> GetOrCreateCartAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow,
                    CartItems = new List<CartItem>()
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        public async Task AddToCartAsync(int plantId, string userId, int quantity)
        {
            // Перевіряємо наявність товару
            var plant = await _plantService.GetPlantByIdAsync(plantId);
            if (plant == null)
                throw new ArgumentException("Товар не знайдено");

            if (!plant.IsAvailable)
                throw new InvalidOperationException("Товар недоступний для покупки");

            // Отримуємо або створюємо кошик
            var cart = await GetOrCreateCartAsync(userId);

            // Перевіряємо, чи є вже такий товар у кошику
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.PlantId == plantId);

            if (cartItem == null)
            {
                // Додаємо новий товар до кошика
                cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    PlantId = plantId,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                // Збільшуємо кількість існуючого товару
                cartItem.Quantity++;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetCartItemsCountAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return 0;

            return cart.CartItems.Sum(ci => ci.Quantity);
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart?.CartItems != null)
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<CartItem>> GetCartItemsAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Plant)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            return cart?.CartItems?.ToList() ?? new List<CartItem>();
        }

        public async Task UpdateCartItemQuantityAsync(int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);

            if (cartItem == null)
                throw new ArgumentException("Товар не знайдено в кошику");

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveCartItemAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> GetCartTotalAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Plant)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return 0;

            return cart.CartItems.Sum(ci => ci.Quantity * ci.Plant.CurrentPrice);
        }
    }
}
