using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetMarket.Models
{
    /// <summary>
    /// Модель данных "Статус заказов"
    /// </summary>
    [Table("OrdersStatus")]
    public class OrderStatus
    {
        public int Id { get; set; }

        public string Status { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public OrderStatus()
        {
            Orders = new List<Order>();
        }
    }
}