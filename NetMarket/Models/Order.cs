using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NetMarket.Models
{
    public class Order
    {
        public int Id { get; set; }

        public Guid? UserId { get; set; }

        public DateTime OrderTime { get; set; }

        public int OrderStatusId { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string MiddleName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        [MaybeNull]
        public string Comment { get; set; }

        public int Sum { get; set; }

        public virtual User User { get; set; }

        public virtual OrderStatus OrderStatus { get; set; }

        public virtual ICollection<OrderProduct> OrderProducts { get; set; }

        public Order()
        {
            OrderProducts = new List<OrderProduct>();
        }
    }
}