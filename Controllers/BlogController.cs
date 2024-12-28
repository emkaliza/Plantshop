using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plantshop.Data;
using Plantshop.Models;

namespace Plantshop.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlogController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                .Include(p => p.BlogCategory)
                .Include(p => p.Author)
                .Where(p => p.IsPublished)
                .OrderByDescending(p => p.PublishedAt)
                .ToListAsync();
            return View(posts);
        }

        public async Task<IActionResult> Category(string slug)
        {
            var posts = await _context.Posts
                .Include(p => p.BlogCategory)
                .Include(p => p.Author)
                .Where(p => p.BlogCategory.Slug == slug && p.IsPublished)
                .OrderByDescending(p => p.PublishedAt)
                .ToListAsync();
            return View(posts);
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.BlogCategories.ToListAsync();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post)
        {
            if (ModelState.IsValid)
            {
                post.AuthorId = User.Identity.Name;
                post.PublishedAt = DateTime.UtcNow;
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }
    }
}