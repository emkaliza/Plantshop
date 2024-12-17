using PlantShop.Models;

namespace Plantshop.ViewModels
{
    public class HomeIndexViewModel
    {
        public PaginatedList<Plant>? Plants { get; set; }
        public ICollection<CategoryViewModel>? Categories { get; set; }

        public int? CurrentCategoryId { get; set; }
        public decimal CurrentMinPrice { get; set; }
        public decimal CurrentMaxPrice { get; set; }
        public string CurrentSortOrder { get; set; }
        public bool IsNew { get; set; }
        public bool IsOnSale { get; set; }
        public string? SearchTerm { get; set; }
    }
}
