using System.ComponentModel.DataAnnotations;

namespace Plantshop.Models
{
    public class BlogCategory
    {
        public int BlogCategoryId { get; set; }

        [Required(ErrorMessage = "Назва категорії обов'язкова")]
        [Display(Name = "Назва категорії")]
        [StringLength(100, ErrorMessage = "Назва не може перевищувати 100 символів")]
        public string Name { get; set; }

        [Display(Name = "Опис категорії")]
        public string? Description { get; set; }

        [Display(Name = "Slug для URL")]
        public string? Slug { get; set; }

        // Навігаційна властивість для зв'язку з постами
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
