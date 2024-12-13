using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantShop.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public string? UserId { get; set; }

        [DisplayName("Дата")]
        public DateTime OrderDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DisplayName("Рахунок")]
        public decimal TotalAmount { get; set; }

        [DisplayName("Статус замовлення")]
        public string? Status { get; set; }

        [DisplayName("Адреса")]
        public string? ShippingAddress { get; set; }

        [DisplayName("Статус платежу")]
        public string? PaymentStatus { get; set; }

        [DisplayName("Метод платежу")]
        public string? PaymentMethod { get; set; }

        [DisplayName("Метод доставки")]
        public string? DeliveryMethod { get; set; }

        [ForeignKey("UserId")]
        [DisplayName("Користувач")]
        public virtual AppUser? User { get; set; }
        public virtual ICollection<OrderItem>? OrderItems { get; set; }
    }
}
