using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantShop.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        [ForeignKey("Cart")]
        public int CartId { get; set; }

        [ForeignKey("Plant")]
        public int PlantId { get; set; }
        public int Quantity { get; set; }

        public virtual Cart? Cart { get; set; }
        public virtual Plant? Plant { get; set; }
    }
}
