using System.ComponentModel.DataAnnotations;
using NetMarket.ValidationAttributes;

namespace NetMarket.ViewModels
{
    public class OrderRegistrationViewModel
    {
        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        public string Surname { get; set; }

        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [EmailAddress(ErrorMessage = "Некорректный email!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [PhoneNumberNotNull(ErrorMessage = "Некорректный номер телефона!")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        public string Address { get; set; }

        public string? Comment { get; set; }
    }
}