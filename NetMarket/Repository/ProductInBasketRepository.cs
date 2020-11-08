using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetMarket.Entities;
using NetMarket.Models;
using NetMarket.ViewModels;
using NetMarket.ViewModels.Order;

namespace NetMarket.Repository
{
    public class ProductInBasketRepository
    {
        private NetMarketDbContext _netMarketDbContext;

        public ProductInBasketRepository(NetMarketDbContext netMarketDbContext)
        {
            _netMarketDbContext = netMarketDbContext;
        }

        public async Task AddProductInBasketForAuthorizedUserAsync(Guid userId, int productId)
        {
            var productInBasket = new ProductInBasket
            {
                UserId = userId,
                ProductId = productId
            };
            _netMarketDbContext.ProductsInBasket.Add(productInBasket);
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
            await _netMarketDbContext.SaveChangesAsync();
        }

        public int GetCountProductsInBasketForAuthorizedUser(string username)
        {
            return (from product in _netMarketDbContext.ProductsInBasket
                where product.User.Login == username
                select product).ToList().Count;
        }

        public int GetCountProductsInBasketForNotAuthorizedUser(Guid userId)
        {
            return (from product in _netMarketDbContext.ProductsInBasket
                where product.NotAuthorizedUserId == userId
                select product).ToList().Count;
        }

        public List<ProductInBasketViewModel> GetProductsInCartForAuthorizedUser(string username)
        {
            return (from productInBasket in _netMarketDbContext.ProductsInBasket
                where productInBasket.User.Login == username
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

        public OrderRegistrationViewModel GetDataProductsInCartForAuthorizedUser(string username)
        {
            return new OrderRegistrationViewModel
            {
                OrderIdPhones = new List<int>((from productInBasket in _netMarketDbContext.ProductsInBasket
                    where productInBasket.User.Login == username
                    join product in _netMarketDbContext.Products on productInBasket.ProductId equals product.Id
                    select product.Id).ToList()),
                Sum = _netMarketDbContext.ProductsInBasket.Where(product => product.User.Login == username)
                    .Sum(product => product.Product.Price)
            };
        }

        public OrderRegistrationViewModel GetDataProductsInCartForNotAuthorizedUser(Guid userId)
        {
            return new OrderRegistrationViewModel
            {
                OrderIdPhones = new List<int>((from productInBasket in _netMarketDbContext.ProductsInBasket
                    where productInBasket.NotAuthorizedUserId == userId
                    join product in _netMarketDbContext.Products on productInBasket.ProductId equals product.Id
                    select product.Id).ToList()),
                Sum = _netMarketDbContext.ProductsInBasket.Where(product => product.NotAuthorizedUserId == userId)
                    .Sum(product => product.Product.Price)
            };
        }

        public async Task DeleteProductFromCartAsync(int id)
        {
            var product = _netMarketDbContext.ProductsInBasket.Find(id);
            _netMarketDbContext.ProductsInBasket.Remove(product);
            await _netMarketDbContext.SaveChangesAsync();
        }
    }
}