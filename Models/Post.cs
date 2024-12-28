using PlantShop.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Plantshop.Models
{
    public class Post
    {
        public int PostId { get; set; }

        [Required(ErrorMessage = "Заголовок обов'язковий")]
        [Display(Name = "Заголовок")]
        [StringLength(200, ErrorMessage = "Заголовок не може перевищувати 200 символів")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Контент обов'язковий")]
        [Display(Name = "Контент")]
        public required string Content { get; set; }

        [Display(Name = "Короткий опис")]
        [StringLength(500, ErrorMessage = "Короткий опис не може перевищувати 500 символів")]
        public string? Summary { get; set; }

        [Required(ErrorMessage = "Slug обов'язковий")]
        [Display(Name = "Slug для URL")]
        [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Slug може містити тільки малі літери, цифри та дефіси")]
        public required string Slug { get; set; }

        [Display(Name = "Зображення")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Дата публікації")]
        public DateTime PublishedAt { get; set; }

        [Display(Name = "Дата створення")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Дата оновлення")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Опубліковано")]
        public bool IsPublished { get; set; }

        // Зовнішній ключ
        [Required(ErrorMessage = "Категорія обов'язкова")]
        public int BlogCategoryId { get; set; }

        // Навігаційна властивість
        [ForeignKey("CategoryId")]
        public BlogCategory? BlogCategory { get; set; }

        // Зв'язок з автором (є модель AppUser)
        public string? AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public AppUser? Author { get; set; }
    }
}
