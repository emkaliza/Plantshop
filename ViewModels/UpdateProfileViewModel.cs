using System.ComponentModel.DataAnnotations;

namespace Plantshop.ViewModels
{
    public class UpdateProfileViewModel
    {
        [Required]
        [Display(Name = "Ім'я")]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Прізвище")]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Phone]
        [Display(Name = "Телефон")]
        public string? PhoneNumber { get; set; }
    }
}
