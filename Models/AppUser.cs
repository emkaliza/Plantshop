using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PlantShop.Models
{
    public class AppUser: IdentityUser
    {
        [Required]
        [PersonalData]
        [DisplayName("Ім'я")]
        public string? FirstName { get; set; }

        [Required]
        [PersonalData]
        [DisplayName("Прізвище")]
        public string? LastName { get; set; }

        [Required]
        [PersonalData]
        [DisplayName("Адреса")]
        public string? Address { get; set; }

        [DisplayName("Роль")]
        public string Role { get; set; } = "Customer";

        [PersonalData]
        [DisplayName("Реєстрація")]
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        
        [PersonalData]
        [DisplayName("Останній вхід")]
        public DateTime? LastLoginDate { get; set; }

        public ICollection<Order>? Orders { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public Cart? Cart { get; set; }
        public ICollection<Favorite>? Favorites { get; set; }

        public AppUser()
        {
            RegistrationDate = DateTime.UtcNow;
            Orders = new HashSet<Order>();
            Reviews = new HashSet<Review>();
            Favorites = new HashSet<Favorite>();
        }

    }
}
