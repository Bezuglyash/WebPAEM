using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using NetMarket.Entities;
using NetMarket.Models;

namespace NetMarket.Repository
{
    /// <summary>
    /// Класс-репозиторий для работы с товарами на уровне доступа к данным
    /// </summary>
    public class ProductRepository
    {
        private NetMarketDbContext _netMarketDbContext;
        private readonly Dictionary<string, Func<Product, object, Product>> _actionsUpdate;
        private IMemoryCache _cache;

        public ProductRepository(NetMarketDbContext netMarketDbContext, IMemoryCache cache)
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
            _cache = cache;
        }

        /// <summary>
        /// Метод добваления нового товара
        /// </summary>
        /// <param name="company">Компания производитель</param>
        /// <param name="name">Название</param>
        /// <param name="price">Цена</param>
        /// <param name="storageCard">Карта памяти</param>
        /// <param name="color">Цвет</param>
        /// <param name="operationSystem">Операционная система</param>
        /// <param name="weight">Вес</param>
        /// <param name="description">Описание</param>
        /// <param name="existence">Есть ли в наличии</param>
        /// <param name="imageString">Путь к фотографии на сервере</param>
        /// <returns></returns>
        public async Task AddProductAsync(string company, string name, int price, int storageCard, string color, string operationSystem, int weight, string description, string existence, string imageString)
        {
            _cache.Remove("products");
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
                HaveInStock = existence == "Есть в наличии",
                ImageString = imageString
            };
            _netMarketDbContext.Products.Add(product);
            await _netMarketDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Метод получения списка товаров
        /// </summary>
        /// <returns>Список товаров</returns>
        public List<Product> GetProducts()
        {
            if (!_cache.TryGetValue("products", out List<Product> list))
            {
                list = _netMarketDbContext.Products.ToList();
                _cache.Set("products", list, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(28000)));
            }
            return list;
        }

        /// <summary>
        /// Метод получения товара
        /// </summary>
        /// <param name="id">ID товара</param>
        /// <returns>Товар</returns>
        public Product GetProduct(int id)
        {
            return (from product in GetProducts()
                    where product.Id == id
                select product).ToList()[0];
        }

        /// <summary>
        /// Метод получения списка товаров с учётом поиска
        /// </summary>
        /// <param name="search">Поиск</param>
        /// <returns>Список товаров</returns>
        public List<Product> GetSearchProducts(string search)
        {
            if (int.TryParse(search, out int price))
            {
                return (from product in GetProducts()
                    where product.Price >= price
                    select product).ToList();
            }
            return (from product in GetProducts()
                where product.Name.Contains(search)
                select product).ToList();
        }

        /// <summary>
        /// Метод определяющий, есть ли товар в наличии
        /// </summary>
        /// <param name="id">ID товара</param>
        /// <returns>True, если есть, иначе - false</returns>
        public bool IsHaveInStock(int id)
        {
            var product = (from prod in GetProducts()
                where prod.Id == id
                select prod).ToList()[0];
            return product.HaveInStock;
        }

        /// <summary>
        /// Метод обновления данных о товаре
        /// </summary>
        /// <param name="id">ID товара</param>
        /// <param name="typeOfUpdate">Тип данных, которые нужно обновить (company, name, price, storageCard, color, operationSystem, weight, description, existence)</param>
        /// <param name="data">Обновленные данные</param>
        /// <returns></returns>
        public async Task UpdateAsync(int id, string typeOfUpdate, object data)
        {
            _cache.Remove("products");
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

        /// <summary>
        /// Метод обновления пути к фотографии на сервере (изменения фотографии товара)
        /// </summary>
        /// <param name="id">ID товара</param>
        /// <param name="newImageString">Новый путь</param>
        /// <returns>Путь к прежней фотографии (для удаления из сервера)</returns>
        public async Task<string> ImageStringUpdateAsync(int id, string newImageString)
        {
            _cache.Remove("products");
            var product = _netMarketDbContext.Products.Find(id);
            var currentImageString = product.ImageString;
            product.ImageString = newImageString;
            _netMarketDbContext.Products.Update(product);
            await _netMarketDbContext.SaveChangesAsync();
            return currentImageString;
        }

        /// <summary>
        /// Метод удаления товара
        /// </summary>
        /// <param name="id">ID продукта</param>
        /// <returns></returns>
        public async Task DeleteProductAsync(int id)
        {
            _cache.Remove("products");
            var product = _netMarketDbContext.Products.Find(id);
            _netMarketDbContext.Products.Remove(product);
            await _netMarketDbContext.SaveChangesAsync();
        }
    }
}