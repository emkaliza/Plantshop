﻿using System.ComponentModel.DataAnnotations;

namespace Plantshop.ViewModels
{
    public class UpdateProfileViewModel
    {
        [Required(ErrorMessage = "Поле Ім'я обов'язкове")]
        [Display(Name = "Ім'я")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Поле Прізвище обов'язкове")]
        [Display(Name = "Прізвище")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Поле Email обов'язкове")]
        [EmailAddress(ErrorMessage = "Некоректний формат Email")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Некоректний формат телефону")]
        [Display(Name = "Телефон")]
        public string? PhoneNumber { get; set; }
    }
}
