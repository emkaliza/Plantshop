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

        public async Task<IActionResult> Index(int? pageNumber)
        {
            int pageSize = 6;

            var plants = _context.Plants
                        .Include(p => p.Category)
                        .Include(p => p.Discount)
                        .AsNoTracking();

            var viewModel = new HomeIndexViewModel
            {
                Plants = await PaginatedList<Plant>.CreateAsync(
                    plants,
                    pageNumber ?? 1,
                    pageSize
                ),
                Categories = await _context.Categories.Include(c => c.Plants).ToListAsync()
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
