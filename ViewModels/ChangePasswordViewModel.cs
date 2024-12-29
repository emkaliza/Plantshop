using System.ComponentModel.DataAnnotations;

namespace Plantshop.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Введіть поточний пароль")]
        [Display(Name = "Поточний пароль")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Введіть новий пароль")]
        [Display(Name = "Новий пароль")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль повинен містити мінімум 6 символів")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Підтвердження паролю")]
        [Compare("NewPassword", ErrorMessage = "Паролі не співпадають")]
        public string ConfirmPassword { get; set; }
    }
}
