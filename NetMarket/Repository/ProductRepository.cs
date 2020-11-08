using System.Collections.Generic;
using System.Linq;
using NetMarket.Entities;
using NetMarket.Models;

namespace NetMarket.Repository
{
    public class ProductRepository
    {
        private NetMarketDbContext _netMarketDbContext;

        public ProductRepository(NetMarketDbContext netMarketDbContext)
        {
            _netMarketDbContext = netMarketDbContext;
        }

        public void AddProduct(string company, string name, int price, int storageCard, string color, string operationSystem, int weight, string description, bool haveInStock, string imageString)
        {
            Product product = new Product
            {
                Company = company,
                Name = name,
                Price = price,
                StorageCard = storageCard,
                Color = color,
                OperationSystem = operationSystem,
                Weight = weight,
                Description = description,
                HaveInStock = haveInStock,
                ImageString = imageString
            };
            _netMarketDbContext.Products.Add(product);
            _netMarketDbContext.SaveChanges();
        }

        public List<Product> GetProducts()
        {
            return _netMarketDbContext.Products.ToList();
        }

        public Product GetProduct(int id)
        {
            return (from product in _netMarketDbContext.Products
                where product.Id == id
                select product).ToList()[0];
        }
    }
}