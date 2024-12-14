using PlantShop.Models;

namespace Plantshop.ViewModels
{
    public class HomeIndexViewModel
    {
        public PaginatedList<Plant> Plants { get; set; }
        public ICollection<Category> Categories { get; set; }
    }
}
