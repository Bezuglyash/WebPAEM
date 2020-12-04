using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetMarket.Models
{
    [Table("ProductsInBasket")]
    public class ProductInBasket
    {
        public int Id { get; set; }

        public Guid? UserId { get; set; }

        public Guid? NotAuthorizedUserId { get; set; }

        public int ProductId { get; set; }

        public virtual People User { get; set; }

        public virtual Product Product { get; set; }
    }
}