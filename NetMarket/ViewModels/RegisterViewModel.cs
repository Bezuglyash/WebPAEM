using System.ComponentModel.DataAnnotations;

namespace NetMarket.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Пароли не совпадают!")]
        public string RepeatPassword { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        public string MiddleName { get; set; }

        public string PhoneNumber { get; set; }
    }
}