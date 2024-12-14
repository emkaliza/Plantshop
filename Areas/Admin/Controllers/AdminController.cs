using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plantshop.Data;

namespace PlantShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : AdminBaseController
    {
        private readonly ApplicationDbContext? _context;
        public IActionResult Index()
        {
            return View();
        }
    }
}
