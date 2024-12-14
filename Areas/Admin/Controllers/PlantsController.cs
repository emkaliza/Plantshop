using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Plantshop.Data;
using PlantShop.Models;

namespace PlantShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PlantsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PlantsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Plants
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var pageSize = 5;

            var plants = _context.Plants.Include(p => p.Category).Include(p => p.Discount).AsQueryable();

            return View(await PaginatedList<Plant>.CreateAsync(plants, pageNumber ?? 1, pageSize));
        }

        // GET: Admin/Plants/Details/{PlantId}
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plant = await _context.Plants
                .Include(p => p.Category)
                .Include(p => p.Discount)
                .FirstOrDefaultAsync(m => m.PlantId == id);
            if (plant == null)
            {
                return NotFound();
            }

            return View(plant);
        }

        // GET: Admin/Plants/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");
            ViewData["DiscountId"] = new SelectList(_context.Discounts, "DiscountId", "Name");
            return View();
        }

        // POST: Admin/Plants/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlantId,Name,Description,BasePrice,StockQuantity,CategoryId,IsAvailable,DiscountId,IsOnSale")] Plant plant, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                plant.CreatedDate = DateTime.Now;
                plant.LastModifiedDate = DateTime.Now;

                if (imageFile != null && imageFile.Length > 0)
                {
                    var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                    if (!allowedTypes.Contains(imageFile.ContentType.ToLower()))
                    {
                        ModelState.AddModelError("ImageUrl", "Допускаються тільки зображення (jpg, jpeg, png, gif)");
                        PrepareViewData(plant);
                        return View(plant);
                    }

                    if (imageFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("ImageUrl", "Розмір файлу не повинен перевищувати 5MB");
                        PrepareViewData(plant);
                        return View(plant);
                    }

                    try
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "plants");

                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        string filePath = Path.Combine(uploadsFolder, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        plant.ImageUrl = Path.Combine("img", "plants", fileName);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("ImageUrl", $"Помилка під час завантаження зображення: {ex.Message}");
                        PrepareViewData(plant);
                        return View(plant);
                    }
                }

                if (plant.DiscountId.HasValue)
                {
                    var discount = await _context.Discounts.FindAsync(plant.DiscountId);
                    if (discount != null && plant.BasePrice.HasValue)
                    {
                        plant.DiscountPrice = plant.BasePrice.Value * (1 - (decimal)discount.DiscountPercentage / 100);
                    }
                }
                else
                {
                    plant.DiscountPrice = null;
                    plant.IsOnSale = false;
                }

                try
                {
                    _context.Add(plant);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Помилка під час збереження: {ex.Message}");
                    PrepareViewData(plant);
                    return View(plant);
                }
            }

            PrepareViewData(plant);
            return View(plant);
        }

        // GET: Admin/Plants/Edit/{PlantId}
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plant = await _context.Plants.FindAsync(id);
            if (plant == null)
            {
                return NotFound();
            }

            PrepareViewData(plant);
            return View(plant);
        }

        // POST: Admin/Plants/Edit/{PlantId}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlantId,Name,Description,BasePrice,StockQuantity,CategoryId,IsAvailable,DiscountId,IsOnSale")] Plant plant, IFormFile? imageFile)
        {
            if (id != plant.PlantId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPlant = await _context.Plants.AsNoTracking()
                        .FirstOrDefaultAsync(p => p.PlantId == id);

                    if (existingPlant == null)
                    {
                        return NotFound();
                    }

                    plant.CreatedDate = existingPlant.CreatedDate;
                    plant.ImageUrl = existingPlant.ImageUrl;
                    plant.LastModifiedDate = DateTime.Now;

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                        if (!allowedTypes.Contains(imageFile.ContentType.ToLower()))
                        {
                            ModelState.AddModelError("ImageUrl", "Допускаються тільки зображення (jpg, jpeg, png, gif)");
                            PrepareViewData(plant);
                            return View(plant);
                        }

                        if (imageFile.Length > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("ImageUrl", "Розмір файлу не повинен перевищувати 5MB");
                            PrepareViewData(plant);
                            return View(plant);
                        }

                        try
                        {
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "plants");

                            if (!Directory.Exists(uploadsFolder))
                                Directory.CreateDirectory(uploadsFolder);

                            string filePath = Path.Combine(uploadsFolder, fileName);

                            if (!string.IsNullOrEmpty(existingPlant.ImageUrl))
                            {
                                string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingPlant.ImageUrl);
                                if (System.IO.File.Exists(oldFilePath))
                                {
                                    System.IO.File.Delete(oldFilePath);
                                }
                            }

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await imageFile.CopyToAsync(fileStream);
                            }

                            plant.ImageUrl = Path.Combine("img", "plants", fileName);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("ImageUrl", $"Помилка під час завантаження зображення: {ex.Message}");
                            PrepareViewData(plant);
                            return View(plant);
                        }
                    }

                    if (plant.DiscountId.HasValue)
                    {
                        var discount = await _context.Discounts.FindAsync(plant.DiscountId);
                        if (discount != null && plant.BasePrice.HasValue)
                        {
                            plant.DiscountPrice = plant.BasePrice.Value * (1 - (decimal)discount.DiscountPercentage / 100);
                        }
                    }
                    else
                    {
                        plant.DiscountPrice = null;
                        plant.IsOnSale = false;
                    }

                    _context.Update(plant);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlantExists(plant.PlantId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Помилка під час збереження: {ex.Message}");
                    PrepareViewData(plant);
                    return View(plant);
                }
            }

            PrepareViewData(plant);
            return View(plant);
        }

        // Допоміжний метод для підготовки ViewData
        private void PrepareViewData(Plant plant)
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", plant.CategoryId);
            ViewData["DiscountId"] = new SelectList(_context.Discounts, "DiscountId", "Name", plant.DiscountId);
        }

        // POST: Admin/Plants/Delete/{PlantId}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plant = await _context.Plants.FindAsync(id);
            if (plant != null)
            {
                _context.Plants.Remove(plant);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlantExists(int id)
        {
            return _context.Plants.Any(e => e.PlantId == id);
        }
    }
}
