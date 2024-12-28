using PlantShop.Models;

namespace Plantshop.ViewModels
{
    public class BlogIndexViewModel
    {
        public PaginatedList<Models.Post>? Posts { get; set; }
        public IEnumerable<Models.BlogCategory> BlogCategories { get; set; } = [];
        public int? CurrentCategoryId { get; set; }
    }
}
