using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetMarket.Entities;
using NetMarket.Models;

namespace NetMarket.Repository
{
    public class ProductRepository
    {
        private NetMarketDbContext _netMarketDbContext;
        private readonly Dictionary<string, Func<Product, object, Product>> _actionsUpdate;

        public ProductRepository(NetMarketDbContext netMarketDbContext)
        {
            _netMarketDbContext = netMarketDbContext;
            _actionsUpdate = new Dictionary<string, Func<Product, object, Product>>();
            _actionsUpdate.Add("company", CompanyUpdate);
            _actionsUpdate.Add("name", NameUpdate);
            _actionsUpdate.Add("price", PriceUpdate);
            _actionsUpdate.Add("storageCard", StorageCardUpdate);
            _actionsUpdate.Add("color", ColorUpdate);
            _actionsUpdate.Add("operationSystem", OperationSystemUpdate);
            _actionsUpdate.Add("weight", WeightUpdate);
            _actionsUpdate.Add("description", DescriptionUpdate);
            _actionsUpdate.Add("existence", ExistenceUpdate);
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

        public async Task UpdateAsync(int id, string typeOfUpdate, object data)
        {
            var product = _netMarketDbContext.Products.Find(id);
            _netMarketDbContext.Products.Update(_actionsUpdate[typeOfUpdate](_netMarketDbContext.Products.Find(id), data));
            await _netMarketDbContext.SaveChangesAsync();
        }

        private Product CompanyUpdate(Product product, object newData)
        {
            product.Company = (string)newData;
            return product;
        }

        private Product NameUpdate(Product product, object newData)
        {
            product.Name = (string)newData;
            return product;
        }

        private Product PriceUpdate(Product product, object newData)
        {
            product.Price = (int)newData;
            return product;
        }

        private Product StorageCardUpdate(Product product, object newData)
        {
            product.StorageCard = (int)newData;
            return product;
        }

        private Product ColorUpdate(Product product, object newData)
        {
            product.Color = (string)newData;
            return product;
        }

        private Product OperationSystemUpdate(Product product, object newData)
        {
            product.OperationSystem = (string)newData;
            return product;
        }

        private Product WeightUpdate(Product product, object newData)
        {
            product.Weight = (int)newData;
            return product;
        }

        private Product DescriptionUpdate(Product product, object newData)
        {
            product.Description = (string)newData;
            return product;
        }

        private Product ExistenceUpdate(Product product, object newData)
        {
            product.HaveInStock = (string)newData == "Есть в наличии";
            return product;
        }

        public async Task<string> ImageStringUpdateAsync(int id, string newImageString)
        {
            var product = _netMarketDbContext.Products.Find(id);
            var currentImageString = product.ImageString;
            product.ImageString = newImageString;
            _netMarketDbContext.Products.Update(product);
            await _netMarketDbContext.SaveChangesAsync();
            return currentImageString;
        }
    }
}