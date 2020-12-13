using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using NetMarket.Entities;
using NetMarket.Models;
using NetMarket.ViewModels;

namespace NetMarket.Repository
{
    /// <summary>
    /// Класс-репозиторий для работы с товарами в корзине на уровне доступа к данным
    /// </summary>
    public class ProductInBasketRepository
    {
        private NetMarketDbContext _netMarketDbContext;
        private IMemoryCache _cache;

        public ProductInBasketRepository(NetMarketDbContext netMarketDbContext, IMemoryCache cache)
        {
            _netMarketDbContext = netMarketDbContext;
            _cache = cache;
        }

        /// <summary>
        /// Метод добавления товара в корзину авторизированного пользователя
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="userId">ID пользователя</param>
        /// <param name="productId">ID товара</param>
        /// <returns></returns>
        public async Task AddProductInBasketForAuthorizedUserAsync(string login, Guid userId, int productId)
        {
            var productInBasket = new ProductInBasket
            {
                UserId = userId,
                ProductId = productId
            };
            _netMarketDbContext.ProductsInBasket.Add(productInBasket);
            _cache.Remove(login);
            await _netMarketDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Метод добавления товара в корзину неавторизированного пользователя
        /// </summary>
        /// <param name="userId">ID неавторизированного пользователя</param>
        /// <param name="productId">ID товара</param>
        /// <returns></returns>
        public async Task AddProductInBasketForNotAuthorizedUserAsync(Guid userId, int productId)
        {
            var productInBasket = new ProductInBasket
            {
                NotAuthorizedUserId = userId,
                ProductId = productId
            };
            _netMarketDbContext.ProductsInBasket.Add(productInBasket);
            _cache.Remove(userId.ToString());
            await _netMarketDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Метод подсчёта количества товаров в корзине авторизированного пользователя
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <returns>Количество товаров</returns>
        public int GetCountProductsInBasketForAuthorizedUser(string login)
        {
            return (from product in _netMarketDbContext.ProductsInBasket
                where product.User.Login == login
                select product).ToList().Count;
        }

        /// <summary>
        /// Метод подсчёта количества товаров в корзине неавторизированного пользователя
        /// </summary>
        /// <param name="userId">ID неавторизированного пользователя</param>
        /// <returns>Количество товаров</returns>
        public int GetCountProductsInBasketForNotAuthorizedUser(Guid userId)
        {
            return (from product in _netMarketDbContext.ProductsInBasket
                where product.NotAuthorizedUserId == userId
                select product).ToList().Count;
        }

        /// <summary>
        /// Метод получения списка товаров в корзине авторизированного пользователя
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <returns>Список товаров</returns>
        public List<ProductInBasketViewModel> GetProductsInCartForAuthorizedUser(string login)
        {
            return (from productInBasket in _netMarketDbContext.ProductsInBasket
                where productInBasket.User.Login == login
                join product in _netMarketDbContext.Products on productInBasket.ProductId equals product.Id 
                select new ProductInBasketViewModel {
                    Id = productInBasket.Id,
                    ProductId = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    ImageString = product.ImageString
                }).ToList();
        }

        /// <summary>
        /// Метод получения списка товаров в корзине неавторизированного пользователя
        /// </summary>
        /// <param name="userId">ID неавторизированного пользователя</param>
        /// <returns>Список товаров</returns>
        public List<ProductInBasketViewModel> GetProductsInCartForNotAuthorizedUser(Guid userId)
        {
            return (from productInBasket in _netMarketDbContext.ProductsInBasket
                where productInBasket.NotAuthorizedUserId == userId
                join product in _netMarketDbContext.Products on productInBasket.ProductId equals product.Id
                select new ProductInBasketViewModel
                {
                    Id = productInBasket.Id,
                    ProductId = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    ImageString = product.ImageString
                }).ToList();
        }

        /// <summary>
        /// Метод подсчитывающий сумму заказа авторизированного пользователя
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <returns>Сумма заказа</returns>
        public int GetPriceSumProductsInCartForAuthorizedUser(string login)
        {
            if (!_cache.TryGetValue(login, out int sum))
            {
                sum = _netMarketDbContext.ProductsInBasket.Where(product => product.User.Login == login).Sum(product => product.Product.Price);
                _cache.Set(login, sum, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(3)));
            }
            return sum;
        }

        /// <summary>
        /// Метод подсчитывающий сумму заказа неавторизированного пользователя
        /// </summary>
        /// <param name="userId">ID неавторизированного пользователя</param>
        /// <returns>Сумма заказа</returns>
        public int GetPriceSumProductsInCartForNotAuthorizedUser(Guid userId)
        {
            if (!_cache.TryGetValue(userId.ToString(), out int sum))
            {
                sum = _netMarketDbContext.ProductsInBasket.Where(product => product.NotAuthorizedUserId == userId).Sum(product => product.Product.Price);
                _cache.Set(userId.ToString(), sum, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(3)));
            }
            return sum;
        }

        /// <summary>
        /// Метод удаления товара из корзины
        /// </summary>
        /// <param name="id">ID корзины</param>
        /// <returns></returns>
        public async Task DeleteProductFromCartAsync(int id)
        {
            var product = _netMarketDbContext.ProductsInBasket.Find(id);
            _cache.Remove(product.UserId != null ? product.User.Login : product.NotAuthorizedUserId.ToString());
            _netMarketDbContext.ProductsInBasket.Remove(product);
            await _netMarketDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Метод удаления товаров из корзины авторизированного пользователя
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <returns>Список ID товаров</returns>
        public async Task<List<int>> DeleteProductsInBasketForAuthorizedUserAsync(string login)
        {
            var products = (from p in _netMarketDbContext.ProductsInBasket
                where p.User.Login == login
                select p).ToList();
            var productsId = new List<int>();
            foreach (var product in products)
            {
                productsId.Add(product.ProductId);
                _netMarketDbContext.ProductsInBasket.Remove(product);
            }
            _cache.Remove(login);
            await _netMarketDbContext.SaveChangesAsync();
            return productsId;
        }

        /// <summary>
        /// Метод удаления товаров из корзины неавторизированного пользователя
        /// </summary>
        /// <param name="userId">ID неавторизированного пользователя</param>
        /// <returns>Список ID товаров</returns>
        public async Task<List<int>> DeleteProductsInBasketForNotAuthorizedUserAsync(Guid userId)
        {
            var products = (from p in _netMarketDbContext.ProductsInBasket
                where p.NotAuthorizedUserId == userId
                select p).ToList();
            var productsId = new List<int>();
            foreach (var product in products)
            {
                productsId.Add(product.ProductId);
                _netMarketDbContext.ProductsInBasket.Remove(product);
            }
            _cache.Remove(userId.ToString());
            await _netMarketDbContext.SaveChangesAsync();
            return productsId;
        }
        
        /// <summary>
        /// Метод удаления товара из корзины, которого нет в наличии
        /// </summary>
        /// <param name="productId">ID товара</param>
        /// <returns></returns>
        public async Task DeleteAllProductsThatAreOutOfStockAsync(int productId)
        {
            var products = (from p in _netMarketDbContext.ProductsInBasket
                where p.ProductId == productId
                select p).ToList();
            foreach (var product in products)
            {
                _netMarketDbContext.ProductsInBasket.Remove(product);
            }
            await _netMarketDbContext.SaveChangesAsync();
        }
    }
}