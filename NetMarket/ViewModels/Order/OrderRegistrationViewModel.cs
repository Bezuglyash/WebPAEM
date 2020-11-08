using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetMarket.ViewModels.Order
{
    public class OrderRegistrationViewModel
    {
        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        [EmailAddress(ErrorMessage = "Некорректный электронный адрес")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Это поле обязательно для заполнения!")]
        public string Address { get; set; }

        public string Description { get; set; }

        public List<int> OrderIdPhones { get; set; }

        public int Sum { get; set; }
    }
}