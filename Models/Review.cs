using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantShop.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [ForeignKey("Plant")]
        public int PlantId { get; set; }

        [Required]
        public string? UserId { get; set; }

        [Range(1,5)]
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual Plant? Plant { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser? User { get; set; }
    }
}
