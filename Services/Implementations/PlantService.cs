using Plantshop.Data;
using PlantShop.Models;
using Plantshop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Plantshop.Services.Implementations
{
    public class PlantService : IPlantService
    {
        private readonly ApplicationDbContext _context;

        public PlantService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Plant> GetPlantByIdAsync(int plantId)
        {
            return await _context.Plants
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.PlantId == plantId);
        }

        public async Task<List<Plant>> GetAllPlantsAsync()
        {
            return await _context.Plants
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<List<Plant>> GetAvailablePlantsAsync()
        {
            return await _context.Plants
                .Include(p => p.Category)
                .Where(p => p.IsAvailable && p.StockQuantity > 0)
                .ToListAsync();
        }

        public async Task<bool> IsPlantAvailableAsync(int plantId)
        {
            var plant = await _context.Plants
                .FirstOrDefaultAsync(p => p.PlantId == plantId);
            return plant != null && plant.IsAvailable && plant.StockQuantity > 0;
        }

        public async Task<Plant> AddPlantAsync(Plant plant)
        {
            _context.Plants.Add(plant);
            await _context.SaveChangesAsync();
            return plant;
        }

        public async Task UpdatePlantAsync(Plant plant)
        {
            var existingPlant = await _context.Plants.FindAsync(plant.PlantId);
            if (existingPlant == null)
                throw new ArgumentException($"Рослину з ID {plant.PlantId} не знайдено");

            _context.Entry(existingPlant).CurrentValues.SetValues(plant);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePlantAsync(int plantId)
        {
            var plant = await _context.Plants.FindAsync(plantId);
            if (plant != null)
            {
                _context.Plants.Remove(plant);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Plant>> GetPlantsByCategoryAsync(int categoryId)
        {
            return await _context.Plants
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<bool> UpdatePlantStockAsync(int plantId, int quantity)
        {
            var plant = await _context.Plants.FindAsync(plantId);
            if (plant == null)
                return false;

            // Перевіряємо, чи не буде від'ємна кількість
            if (plant.StockQuantity + quantity < 0)
                return false;

            plant.StockQuantity += quantity;
            // Якщо кількість стала 0, можна автоматично встановити IsAvailable в false
            plant.IsAvailable = plant.StockQuantity > 0;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
