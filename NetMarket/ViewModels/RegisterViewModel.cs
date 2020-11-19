using System.ComponentModel.DataAnnotations;
using NetMarket.ValidationAttributes;

namespace NetMarket.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [LoginLength(ErrorMessage = "Количество символов не должно превышать 12!")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [EmailAddress(ErrorMessage = "Некорректный email!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!"), Compare("Password", ErrorMessage = "Пароли не совпадают!")]
        public string RepeatPassword { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        public string Surname { get; set; }

        public string? MiddleName { get; set; }

        public string PhoneNumber { get; set; }
    }
}