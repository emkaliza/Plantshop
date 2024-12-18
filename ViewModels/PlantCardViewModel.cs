namespace Plantshop.ViewModels
{
    public class PlantCardViewModel
    {
        public int PlantId { get; set; }
        public string? Name { get; set; }
        public decimal? BasePrice { get; set; }
        public decimal? DiscountPrice { get; set; }
        public bool IsOnSale { get; set; }
        public string? ImageUrl { get; set; }
    }
}