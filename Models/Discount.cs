using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PlantShop.Models
{
    public class Discount
    {
        [Key]
        public int DiscountId { get; set; }

        [DisplayName("Назва")]
        public string Name { get; set; }

        [DisplayName("Код")]
        public string Code { get; set; }

        [DisplayName("Опис")]
        public string Description { get; set; }

        [Range(0, 100)]
        [DisplayName("Відсоток знижки")]
        public decimal DiscountPercentage { get; set; }

        [DisplayName("Фіксована сума знижки")]
        public decimal? FixedDiscountAmount { get; set; }

        [DisplayName("Дата початку")]
        public DateTime StartDate { get; set; }

        [DisplayName("Дата завершення")]
        public DateTime EndDate { get; set; }

        [DisplayName("Активність")]
        public bool IsActive { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DisplayName("Мінімальна сума покупки")]
        public decimal MinimumPurchaseAmount { get; set; }

        [DisplayName("Максимальна кількість використань")]
        public int? MaxUsageCount { get; set; }

        [DisplayName("Поточна кількість використань")]
        public int CurrentUsageCount { get; set; }

        [DisplayName("Максимальна сума знижки")]
        public decimal? MaximumDiscountAmount { get; set; }

        [DisplayName("Створено")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Змінено")]
        public DateTime? LastUsedDate { get; set; }

        public virtual ICollection<Plant> Plants { get; set; }

        // Конструктор
        public Discount()
        {
            Plants = new HashSet<Plant>();
            CreatedDate = DateTime.UtcNow;
            IsActive = true;
        }

        // Метод для перевірки активності знижки
        public bool IsValidNow()
        {
            var now = DateTime.UtcNow;
            return IsActive
                && now >= StartDate
                && now <= EndDate
                && (MaxUsageCount == null || CurrentUsageCount < MaxUsageCount);
        }

        // Метод для розрахунку знижки
        public decimal CalculateDiscount(decimal originalPrice)
        {
            if (FixedDiscountAmount.HasValue)
            {
                return Math.Min(FixedDiscountAmount.Value, originalPrice);
            }

            var discountAmount = originalPrice * (DiscountPercentage / 100);

            if (MaximumDiscountAmount.HasValue)
            {
                discountAmount = Math.Min(discountAmount, MaximumDiscountAmount.Value);
            }

            return discountAmount;
        }
    }
}
