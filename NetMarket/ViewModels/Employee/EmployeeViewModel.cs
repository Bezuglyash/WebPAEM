using System;
using System.ComponentModel.DataAnnotations;
using NetMarket.ValidationAttributes;

namespace NetMarket.ViewModels.Employee
{
    public class EmployeeViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        public string Name { get; set; }

        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [EmailAddress(ErrorMessage = "Некорректный email!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [PhoneNumberNotNull(ErrorMessage = "Некорректный номер телефона!")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        public int Role { get; set; }
    }
}