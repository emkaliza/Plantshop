using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantShop.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [Required]
        public string? UserId { get; set; }
        public DateTime CreatedDate { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser? User { get; set; }
        public virtual ICollection<CartItem>? CartItems { get; set; }
    }
}
