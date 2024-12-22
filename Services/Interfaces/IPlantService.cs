using PlantShop.Models;

namespace Plantshop.Services.Interfaces
{
    public interface IPlantService
    {
        Task<Plant> GetPlantByIdAsync(int plantId);
        Task<List<Plant>> GetAllPlantsAsync();
        Task<List<Plant>> GetAvailablePlantsAsync();
        Task<bool> IsPlantAvailableAsync(int plantId);
        Task<Plant> AddPlantAsync(Plant plant);
        Task UpdatePlantAsync(Plant plant);
        Task DeletePlantAsync(int plantId);
        Task<List<Plant>> GetPlantsByCategoryAsync(int categoryId);
        Task<bool> UpdatePlantStockAsync(int plantId, int quantity);
    }
}
