using PlantShop.Models;

namespace Plantshop.Models
{
    public class Wishlist
    {
        public int WishlistId { get; set; }
        public string? UserId { get; set; }
        public int PlantId { get; set; }
        public DateTime DateAdded { get; set; }

        public AppUser? User { get; set; }
        public Plant? Plant { get; set; }
    }
}
