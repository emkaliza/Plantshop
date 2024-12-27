using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plantshop.Data;
using Plantshop.ViewModels;
using PlantShop.Models;
using System.Security.Claims;

namespace Plantshop.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview(int plantId, int rating, string comment)
        {
            try
            {
                if (rating < 1 || rating > 5)
                {
                    return BadRequest("Неправильний рейтинг");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Перевіряємо чи користувач вже залишав відгук
                var existingReview = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.PlantId == plantId && r.UserId == userId);

                if (existingReview != null)
                {
                    // Оновлюємо існуючий відгук
                    existingReview.Rating = rating;
                    existingReview.Comment = comment;
                    existingReview.CreatedDate = DateTime.UtcNow;
                }
                else
                {
                    // Створюємо новий відгук
                    var review = new Review
                    {
                        PlantId = plantId,
                        UserId = userId,
                        Rating = rating,
                        Comment = comment,
                        CreatedDate = DateTime.UtcNow
                    };

                    _context.Reviews.Add(review);
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetReviews(int plantId)
        {
            ViewBag.PlantId = plantId;  // Додаємо це

            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.PlantId == plantId)
                .OrderByDescending(r => r.CreatedDate)
                .Select(r => new ReviewViewModel
                {
                    UserName = r.User.UserName,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedDate = r.CreatedDate.Value
                })
                .ToListAsync();

            return PartialView("_Reviews", reviews);
        }
    }
}
