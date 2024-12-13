using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantShop.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Поле 'Назва' є обов'язковим")]
        [DisplayName("Назва")]
        [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Довжина назви повинна бути від 2 до 100 символів")]
        [RegularExpression(@"^[А-ЯІЄЇҐа-яієїґ\s\w]+$",
        ErrorMessage = "Назва може містити лише літери, цифри та пробіли")]
        public string Name { get; set; } = string.Empty;

        [DisplayName("Опис категорії")]
        [StringLength(500, ErrorMessage = "Довжина опису не може перевищувати 500 символів")]
        public string? Description { get; set; }
        public virtual ICollection<Plant>? Plants { get; set; }
    }
}
