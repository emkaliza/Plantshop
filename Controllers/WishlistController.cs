using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plantshop.Data;
using Plantshop.Models;
using System.Security.Claims;

namespace Plantshop.Controllers
{
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WishlistController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Додати до списку бажань
        [HttpPost]
        [Authorize] // Тільки для авторизованих користувачів
        public async Task<IActionResult> AddToWishlist(int plantId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Перевіряємо чи товар вже є в списку бажань
            var existingItem = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.PlantId == plantId);

            if (existingItem == null)
            {
                _context.Wishlists.Add(new Wishlist
                {
                    UserId = userId,
                    PlantId = plantId,
                    DateAdded = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        // Видалити зі списку бажань
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveFromWishlist(int plantId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wishlistItem = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.PlantId == plantId);

            if (wishlistItem != null)
            {
                _context.Wishlists.Remove(wishlistItem);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        // Показати список бажань
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wishlistItems = await _context.Wishlists
                .Include(w => w.Plant)
                .Where(w => w.UserId == userId)
                .Select(w => w.Plant)
                .ToListAsync();

            return View(wishlistItems);
        }
    }
}
