using System.ComponentModel.DataAnnotations;

namespace PlantShop.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email є обов'язковим")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        public string Email { get; set; }
    }
}
