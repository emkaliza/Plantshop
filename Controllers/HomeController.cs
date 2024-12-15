using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plantshop.Data;
using Plantshop.ViewModels;
using PlantShop.Models;
using System.Diagnostics;

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

            // Фільтрація за категорією
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            // Фільтрація за ціною
            if(minPrice.HasValue)
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

            int pageSize = 6;

            var viewModel = new HomeIndexViewModel
            {
                Plants = await PaginatedList<Plant>.CreateAsync(query,pageNumber ?? 1,pageSize),
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
                IsOnSale = isOnSale ?? false
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
