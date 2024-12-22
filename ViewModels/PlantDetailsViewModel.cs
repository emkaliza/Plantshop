namespace Plantshop.ViewModels
{
    public class PlantDetailsViewModel
    {
        public int PlantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public decimal? DiscountPrice { get; set; }
        public bool IsOnSale { get; set; }
        public string ImageUrl { get; set; }
        public string CategoryName { get; set; }
        public List<ReviewViewModel> Reviews { get; set; }
        public List<List<PlantCardViewModel>> RelatedPlants { get; set; }

        // Ціна, яка буде відображатися (зі знижкою або базова)
        public decimal CurrentPrice => IsOnSale ? DiscountPrice ?? BasePrice : BasePrice;

        // Властивості для кошика
        public int Quantity { get; set; } = 1; // Кількість товару для додавання
        public int AvailableStock { get; set; } // Доступна кількість
        public bool IsInStock => AvailableStock > 0;

        // Властивості для списку бажань
        public bool IsInWishlist { get; set; }

        // Властивості для кнопок
        public bool CanAddToCart => IsInStock && AvailableStock >= Quantity;
        public bool CanBuyNow => CanAddToCart;

        // Обчислювані властивості для відгуків
        public double AverageRating => (double)(Reviews?.Any() == true
            ? Reviews.Average(r => r.Rating)
            : 0);
        public int ReviewsCount => Reviews?.Count ?? 0;
    }
}
