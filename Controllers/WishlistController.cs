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
        /*private readonly ApplicationDbContext _context;
        private readonly ILogger<WishlistController> _logger;

        public WishlistController(ApplicationDbContext context, ILogger<WishlistController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToWishlist([FromQuery] int plantId)  // Змінено на [FromQuery]
        {
            try
            {
                if (plantId <= 0)
                {
                    return BadRequest(new { success = false, message = "Неправильний ID рослини" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new { success = false, message = "Користувач не авторизований" });
                }

                // Перевіряємо чи існує рослина
                var plant = await _context.Plants.FindAsync(plantId);
                if (plant == null)
                {
                    return NotFound(new { success = false, message = "Рослину не знайдено" });
                }

                // Перевіряємо чи товар вже є в списку бажань
                var existingItem = await _context.Wishlists
                    .FirstOrDefaultAsync(w => w.UserId == userId && w.PlantId == plantId);

                if (existingItem == null)
                {
                    var wishlistItem = new Wishlist
                    {
                        UserId = userId,
                        PlantId = plantId,
                        DateAdded = DateTime.UtcNow
                    };

                    _context.Wishlists.Add(wishlistItem);
                    await _context.SaveChangesAsync();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при додаванні до списку бажань");
                return StatusCode(500, new { success = false, message = "Внутрішня помилка сервера" });
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromWishlist([FromQuery] int plantId)  // Змінено на [FromQuery]
        {
            try
            {
                if (plantId <= 0)
                {
                    return BadRequest(new { success = false, message = "Неправильний ID рослини" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new { success = false, message = "Користувач не авторизований" });
                }

                var wishlistItem = await _context.Wishlists
                    .FirstOrDefaultAsync(w => w.UserId == userId && w.PlantId == plantId);

                if (wishlistItem != null)
                {
                    _context.Wishlists.Remove(wishlistItem);
                    await _context.SaveChangesAsync();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при видаленні зі списку бажань");
                return StatusCode(500, new { success = false, message = "Внутрішня помилка сервера" });
            }
        }*/


        private readonly ApplicationDbContext _context;
        private readonly ILogger<WishlistController> _logger;

        public WishlistController(ApplicationDbContext context, ILogger<WishlistController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToWishlist(int plantId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Перевіряємо чи існує рослина
                var plant = await _context.Plants.FindAsync(plantId);
                if (plant == null)
                {
                    _logger.LogWarning($"Спроба додати неіснуючу рослину {plantId} до списку бажань");
                    return Json(new { success = false, message = "Рослину не знайдено" });
                }

                // Перевіряємо чи товар вже є в списку бажань
                var existingItem = await _context.Wishlists
                    .FirstOrDefaultAsync(w => w.UserId == userId && w.PlantId == plantId);

                if (existingItem == null)
                {
                    var wishlistItem = new Wishlist
                    {
                        UserId = userId,
                        PlantId = plantId,
                        DateAdded = DateTime.UtcNow
                    };

                    _context.Wishlists.Add(wishlistItem);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Користувач {userId} додав рослину {plantId} до списку бажань");
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Помилка при додаванні до списку бажань: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Внутрішня помилка сервера" });
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromWishlist(int plantId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var wishlistItem = await _context.Wishlists
                    .FirstOrDefaultAsync(w => w.UserId == userId && w.PlantId == plantId);

                if (wishlistItem != null)
                {
                    _context.Wishlists.Remove(wishlistItem);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Користувач {userId} видалив рослину {plantId} зі списку бажань");
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Помилка при видаленні зі списку бажань: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Внутрішня помилка сервера" });
            }
        }

        /* private readonly ApplicationDbContext _context;

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
         *//*public async Task<IActionResult> Index()
         {
             var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
             var wishlistItems = await _context.Wishlists
                 .Include(w => w.Plant)
                 .Where(w => w.UserId == userId)
                 .Select(w => w.Plant)
                 .ToListAsync();

             return View(wishlistItems);
         }*//*
         public async Task<IActionResult> Index()
         {
             var plants = await _context.Plants.ToListAsync();

             if (User.Identity.IsAuthenticated)
             {
                 var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                 var wishlistItems = await _context.Wishlists
                     .Where(w => w.UserId == userId)
                     .Select(w => w.PlantId)
                     .ToListAsync();

                 ViewBag.WishlistItems = wishlistItems;
             }

             return View(plants);
         }*/
    }
}
