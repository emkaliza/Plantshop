using System.ComponentModel.DataAnnotations;

namespace PlantShop.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ім'я є обов'язковим")]
        [StringLength(100, ErrorMessage = "Ім'я повинно містити від {2} до {1} символів", MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email є обов'язковим")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль є обов'язковим")]
        [StringLength(100, ErrorMessage = "Пароль повинен містити від {2} до {1} символів", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        public string ConfirmPassword { get; set; }
    }
}
