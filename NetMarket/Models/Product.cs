using System.Collections.Generic;

namespace NetMarket.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Company { get; set; }

        public string Name { get; set; }

        public int Price { get; set; }

        public int StorageCard { get; set; }

        public string Color { get; set; }

        public string OperationSystem { get; set; }

        public int Weight { get; set; }

        public string Description { get; set; }

        public bool HaveInStock { get; set; }

        public string ImageString { get; set; }

        public virtual ICollection<ProductInBasket> ProductsInBasket { get; set; }

        public virtual ICollection<OrderProduct> OrderProducts { get; set; }

        public Product()
        {
            ProductsInBasket = new List<ProductInBasket>();
            OrderProducts = new List<OrderProduct>();
        }
    }
}