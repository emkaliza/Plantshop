using PlantShop.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plantshop.ViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Вкажіть адресу доставки")]
        [DisplayName("Адреса доставки")]
        public string ShippingAddress { get; set; }

        [Required(ErrorMessage = "Виберіть спосіб доставки")]
        [DisplayName("Спосіб доставки")]
        public string DeliveryMethod { get; set; }

        [Required(ErrorMessage = "Виберіть спосіб оплати")]
        [DisplayName("Спосіб оплати")]
        public string PaymentMethod { get; set; }

        public List<CartItem> CartItems { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
