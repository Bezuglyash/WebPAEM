using System;
using System.Collections.Generic;

namespace NetMarket.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string Login { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string? MiddleName { get; set; }

        public string? PhoneNumber { get; set; }

        public int RoleId { get; set; }

        public virtual Role Role { get; set; }

        public virtual ICollection<ProductInBasket> ProductsInBasket { get; set; }

        public User()
        {
            ProductsInBasket = new List<ProductInBasket>();
        }
    }
}