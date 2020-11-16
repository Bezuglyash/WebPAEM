using System.ComponentModel.DataAnnotations.Schema;

namespace NetMarket.Models
{
    [Table("OrderProducts")]
    public class OrderProduct
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public virtual Order Order { get; set; }

        public virtual Product Product { get; set; }
    }
}