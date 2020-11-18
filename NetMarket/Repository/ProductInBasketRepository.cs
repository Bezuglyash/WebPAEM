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
    public class ProductInBasketRepository
    {
        private NetMarketDbContext _netMarketDbContext;
        private IMemoryCache _cache;

        public ProductInBasketRepository(NetMarketDbContext netMarketDbContext, IMemoryCache cache)
        {
            _netMarketDbContext = netMarketDbContext;
            _cache = cache;
        }

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

        public int GetCountProductsInBasketForAuthorizedUser(string login)
        {
            return (from product in _netMarketDbContext.ProductsInBasket
                where product.User.Login == login
                select product).ToList().Count;
        }

        public int GetCountProductsInBasketForNotAuthorizedUser(Guid userId)
        {
            return (from product in _netMarketDbContext.ProductsInBasket
                where product.NotAuthorizedUserId == userId
                select product).ToList().Count;
        }

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

        public int GetPriceSumProductsInCartForAuthorizedUser(string login)
        {
            if (!_cache.TryGetValue(login, out int sum))
            {
                sum = _netMarketDbContext.ProductsInBasket.Where(product => product.User.Login == login).Sum(product => product.Product.Price);
                _cache.Set(login, sum, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(3)));
            }
            return sum;
        }

        public int GetPriceSumProductsInCartForNotAuthorizedUser(Guid userId)
        {
            if (!_cache.TryGetValue(userId.ToString(), out int sum))
            {
                sum = _netMarketDbContext.ProductsInBasket.Where(product => product.NotAuthorizedUserId == userId).Sum(product => product.Product.Price);
                _cache.Set(userId.ToString(), sum, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(3)));
            }
            return sum;
        }

        public async Task DeleteProductFromCartAsync(int id)
        {
            var product = _netMarketDbContext.ProductsInBasket.Find(id);
            _cache.Remove(product.UserId != null ? product.User.Login : product.NotAuthorizedUserId.ToString());
            _netMarketDbContext.ProductsInBasket.Remove(product);
            await _netMarketDbContext.SaveChangesAsync();
        }

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
    }
}