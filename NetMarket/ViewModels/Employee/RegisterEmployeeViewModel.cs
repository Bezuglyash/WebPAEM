using System.ComponentModel.DataAnnotations;
using NetMarket.ValidationAttributes;

namespace NetMarket.ViewModels.Employee
{
    public class RegisterEmployeeViewModel : EmployeeViewModel
    {
        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [LoginLength(ErrorMessage = "Количество символов не должно превышать 12!")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!"), Compare("Password", ErrorMessage = "Пароли не совпадают!")]
        public string RepeatPassword { get; set; }
    }
}