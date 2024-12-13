using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantShop.Models
{
    public class Favorite
    {
        [Key]
        public int FavoriteId { get; set; }

        [Required]
        public string? UserId { get; set; }

        [ForeignKey("Plant")]
        public int PlantId { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser? User {  get; set; }
        public virtual Plant? Plant { get; set; }
    }
}
