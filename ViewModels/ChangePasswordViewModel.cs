using System.ComponentModel.DataAnnotations;

namespace Plantshop.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Поточний пароль є обов'язковим")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Новий пароль є обов'язковим")]
        [StringLength(100, ErrorMessage = "Пароль має бути щонайменше {2} символів", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Підтвердження пароля є обов'язковим")]
        [Compare("NewPassword", ErrorMessage = "Паролі не збігаються")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
