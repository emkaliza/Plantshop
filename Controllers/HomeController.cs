using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Plantshop.Data;
using Plantshop.Models;
using Plantshop.ViewModels;
using PlantShop.Models;
using System.Diagnostics;
using System.Numerics;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Plantshop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(
            string searchTerm,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortOrder,
            bool? isNew,
            bool? isOnSale,
            int? pageNumber)
        {
            var query = _context.Plants
                        .Include(p => p.Category)
                        .Include(p => p.Discount)
                        .AsQueryable();

            query = FilterQuery(query, null, categoryId, minPrice, maxPrice, sortOrder, isNew, isOnSale);

            int pageSize = 6;

            return View(await GetHomeIndexViewModel(query, categoryId, minPrice, maxPrice,
                sortOrder, isNew, isOnSale, null, pageNumber));
        }

        private IQueryable<Plant> FilterQuery(
            IQueryable<Plant> query,
            string? searchTerm,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortOrder,
            bool? isNew,
            bool? isOnSale
            )
        {
            // Пошук
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchTerm) ||
                    p.Description.Contains(searchTerm) ||
                    p.Category.Name.Contains(searchTerm))
                    .AsQueryable();
            }

            // Фільтрація за категорією
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            // Фільтрація за ціною
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.BasePrice >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.BasePrice <= maxPrice.Value);
            }

            // Фільтрація нових надходжень
            if (isNew == true)
            {
                var thirtyDaysAgo = DateTime.UtcNow.AddDays(-5);
                query = query.Where(p => p.CreatedDate >= thirtyDaysAgo);
            }

            // Фільтрація товарів зі знижкою
            if (isOnSale == true)
            {
                query = query.Where(p => p.IsOnSale);
            }

            // Сортування
            query = sortOrder switch
            {
                "price_asc" => query.OrderBy(p => p.IsOnSale ? (double)p.DiscountPrice : (double)p.BasePrice),
                "price_desc" => query.OrderByDescending(p => p.IsOnSale ? (double)p.DiscountPrice : (double)p.BasePrice),
                "name_asc" => query.OrderBy(p => p.Name),
                "name_desc" => query.OrderByDescending(p => p.Name),
                "date_desc" => query.OrderByDescending(p => p.CreatedDate),
                _ => query.OrderByDescending(p => p.CreatedDate)
            };

            return query;
        }

        private async Task<HomeIndexViewModel> GetHomeIndexViewModel(
            IQueryable<Plant> query,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortOrder,
            bool? isNew,
            bool? isOnSale,
            string? searchTerm,
            int? pageNumber)
        {
            int pageSize = 6;

            var posts = await _context.Posts
                .Include(p => p.BlogCategory)
                .Include(p => p.Author)
                .Where(p => p.IsPublished)
                .OrderByDescending(p => p.PublishedAt)
                .Take(4)
                .ToListAsync();

            return new HomeIndexViewModel
            {
                Plants = await PaginatedList<Plant>.CreateAsync(query, pageNumber ?? 1, pageSize),
                Categories = await _context.Categories
                    .Select(c => new CategoryViewModel
                    {
                        CategoryId = c.CategoryId,
                        Name = c.Name,
                        Count = c.Plants.Count
                    })
                    .ToListAsync(),
                CurrentCategoryId = categoryId,
                CurrentMinPrice = minPrice ?? 0,
                CurrentMaxPrice = maxPrice ?? 2000,
                CurrentSortOrder = sortOrder,
                IsNew = isNew ?? false,
                IsOnSale = isOnSale ?? false,
                SearchTerm = searchTerm,
                Posts = posts
            };
        }

        public async Task<IActionResult> Search(
            string searchTerm,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortOrder,
            bool? isNew,
            bool? isOnSale,
            int? pageNumber)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return RedirectToAction("Index");

            var query = _context.Plants
                .Include(p => p.Category)
                .Include(p => p.Discount)
                .AsQueryable();

            query = FilterQuery(query, searchTerm, categoryId, minPrice, maxPrice, sortOrder, isNew, isOnSale);

            return View("Index", await GetHomeIndexViewModel(query, categoryId, minPrice, maxPrice,
        sortOrder, isNew, isOnSale, searchTerm, pageNumber));
        }

        public async Task<IActionResult> PlantDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plant = await _context.Plants
                .Include(p => p.Category)
                .Include(p => p.Discount)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(m => m.PlantId == id);

            if (plant == null)
            {
                return NotFound();
            }

            // Отримуємо інформацію про наявність у списку бажань для поточного користувача
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isInWishlist = false;
            if (userId != null)
            {
                isInWishlist = await _context.Wishlists
                    .AnyAsync(w => w.UserId == userId && w.PlantId == id);
            }

            var viewModel = new PlantDetailsViewModel
            {
                PlantId = plant.PlantId,
                Name = plant.Name,
                Description = plant.Description,
                BasePrice = (decimal)plant.BasePrice,
                DiscountPrice = plant.DiscountPrice,
                IsOnSale = plant.IsOnSale,
                ImageUrl = plant.ImageUrl,
                CategoryName = plant.Category.Name,
                AvailableStock = plant.StockQuantity,
                IsInWishlist = isInWishlist,
                Reviews = plant.Reviews.Select(r => new ReviewViewModel
                {
                    UserName = r.UserId.ToString(),
                    Rating = (int)r.Rating,
                    Comment = r.Comment,
                    CreatedDate = (DateTime)r.CreatedDate
                }).ToList(),
                RelatedPlants = await GetRelatedPlants(plant.CategoryId, plant.PlantId)
            };

            return View(viewModel);
        }

        // Приватний метод для отримання схожих рослин
        private async Task<List<List<PlantCardViewModel>>> GetRelatedPlants(int categoryId, int currentPlantId)
        {
            var plants = await _context.Plants
                .Where(p => p.CategoryId == categoryId && p.PlantId != currentPlantId)
                .Take(15)
                .Select(p => new PlantCardViewModel
                {
                    PlantId = p.PlantId,
                    Name = p.Name,
                    BasePrice = (decimal)p.BasePrice,
                    DiscountPrice = (decimal)p.DiscountPrice,
                    IsOnSale = p.IsOnSale,
                    ImageUrl = p.ImageUrl
                })
                .ToListAsync();

            return plants
           .Select((plant, index) => new { plant, index })
           .GroupBy(x => x.index / 5)
           .Take(3) // Не більше трьох груп
           .Select(group => group.Select(x => x.plant).ToList())
           .ToList();

        }

        public IActionResult PlantCare()
        {
            return View();
        }

        public async Task<IActionResult> Blog(int? pageNumber, int? categoryId)
        {
            var pageSize = 3;

            var query = _context.Posts
                .Include(p => p.BlogCategory)
                .Include(p => p.Author)
                .Where(p => p.IsPublished)
                .OrderByDescending(p => p.PublishedAt)
                .AsQueryable();

            // Фільтрація за категорією
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.BlogCategoryId == categoryId.Value);
            }

            var viewModel = new BlogIndexViewModel
            {
                Posts = await PaginatedList<Post>.CreateAsync(query, pageNumber ?? 1, pageSize),
                BlogCategories = await _context.BlogCategories.ToListAsync()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Post(string slug)
        {
            var post = await _context.Posts
                .Include(p => p.BlogCategory)
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished);

            if (post == null) return NotFound();
            return View(post);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
