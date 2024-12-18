using Plantshop.Models;
using PlantShop.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing.Printing;

namespace PlantShop.Models
{
    public class Plant
    {
        [Key]
        public int PlantId { get; set; }

        [Required(ErrorMessage = "Поле 'Назва' є обов'язковим")]
        [DisplayName("Назва")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Довжина назви повинна бути від 2 до 100 символів")]
        [RegularExpression(@"^[А-ЯІЄЇҐа-яієїґ\s\w]+$",
            ErrorMessage = "Назва може містити лише літери, цифри та пробіли")]
        public string? Name { get; set; }

        [DisplayName("Опис")]
        [StringLength(1000, ErrorMessage = "Довжина опису не може перевищувати 1000 символів")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Поле 'Ціна' є обов'язковим")]
        [Column(TypeName = "decimal(18,2)")]
        [DisplayName("Ціна")]
        [Range(0.01, 999999.99,
            ErrorMessage = "Ціна повинна бути більше 0 та менше 1,000,000")]
        public decimal? BasePrice { get; set; }

        [Required(ErrorMessage = "Поле 'Кількість' є обов'язковим")]
        [DisplayName("Кількість")]
        [Range(0, 100,
            ErrorMessage = "Кількість повинна бути від 0 до 100")]
        public int StockQuantity { get; set; }

        [DisplayName("Фото")]
        [Url(ErrorMessage = "Некоректний формат URL")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Поле 'Категорія' є обов'язковим")]
        [ForeignKey("Category")]
        [DisplayName("Категорія")]
        public int CategoryId { get; set; }

        [DisplayName("Доступність")]
        public bool IsAvailable { get; set; } = true;

        [DisplayName("Створено")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Змінено")]
        public DateTime? LastModifiedDate { get; set; }

        [DisplayName("Знижка")]
        public int? DiscountId { get; set; }

        [DisplayName("Ціна зі знижкою")]
        [Range(0, 999999.99,
            ErrorMessage = "Ціна зі знижкою повинна бути більше або дорівнювати 0 та менше 1,000,000")]
        public decimal? DiscountPrice { get; set; }

        [DisplayName("Акція")]
        public bool IsOnSale { get; set; }

        [DisplayName("Знижка")]
        public virtual Discount? Discount { get; set; }

        [DisplayName("Категорія")]
        public virtual Category? Category { get; set; } 
        public virtual ICollection<Review>? Reviews { get; set; }
        public virtual ICollection<OrderItem>? OrderItems { get; set; }
        public virtual ICollection<CartItem>? CartItems { get; set; }
        public virtual ICollection<Favorite>? Favorites { get; set; }
        public virtual ICollection<Wishlist>? Wishlists { get; set; }

        public decimal CurrentPrice
        {
            get
            {
                if (IsOnSale && DiscountPrice.HasValue)
                    return DiscountPrice.Value;
                return BasePrice.Value;
            }
        }
    }
}
