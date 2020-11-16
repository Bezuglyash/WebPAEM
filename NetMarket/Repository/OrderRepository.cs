using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using NetMarket.Entities;
using NetMarket.Models;
using NetMarket.ViewModels.MyOrders;

namespace NetMarket.Repository
{
    public class OrderRepository
    {
        private NetMarketDbContext _netMarketDbContext;
        private IMemoryCache _cache;

        public OrderRepository(NetMarketDbContext netMarketDbContext, IMemoryCache cache)
        {
            _netMarketDbContext = netMarketDbContext;
            _cache = cache;
        }

        public async Task AddNewOrderAsync(Guid? userId, DateTime time, string name, string surname, string middleName, string email, string phoneNumber, string address, string comment, int sum, List<int> productsId)
        {
            var order = new Order
            {
                UserId = userId,
                OrderTime = time,
                OrderStatusId = 1,
                Name = name,
                Surname = surname,
                MiddleName = middleName,
                Email = email,
                PhoneNumber = phoneNumber,
                Address = address,
                Comment = comment,
                Sum = sum
            };
            _netMarketDbContext.Orders.Add(order);

            foreach (var id in productsId)
            {
                var orderProduct = new OrderProduct
                {
                    Order = order,
                    ProductId = id
                };
                _netMarketDbContext.OrderProducts.Add(orderProduct);
            }

            await _netMarketDbContext.SaveChangesAsync();
        }

        public List<OrderViewModel> GetAllUserOrders(string login)
        {
            return (from order in _netMarketDbContext.Orders
                where order.User.Login == login
                select new OrderViewModel
                {
                    OrderNumber = order.Id,
                    CustomerFullName = order.Surname + " " + order.Name + " " + order.MiddleName,
                    Address = order.Address,
                    OrderDate = order.OrderTime.ToShortDateString() + " в " + order.OrderTime.ToShortTimeString(),
                    Sum = order.Sum,
                    Status = (from st in _netMarketDbContext.OrdersStatus
                              join ord in _netMarketDbContext.Orders on st.Id equals ord.OrderStatusId
                              where st.Id == order.OrderStatusId
                              select st.Status).ToList()[0],
                    Comment = order.Comment ?? "-"
                }).ToList();
        }

        public List<ProductInOrderViewModel> GetProductsInOrder(int id)
        {
            if (!_cache.TryGetValue(id, out List<ProductInOrderViewModel> list))
            {
                list = (from order in _netMarketDbContext.Orders
                    join orderProduct in _netMarketDbContext.OrderProducts on order.Id equals orderProduct.OrderId
                    join product in _netMarketDbContext.Products on orderProduct.ProductId equals product.Id
                    where order.Id == id
                    select new ProductInOrderViewModel
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        ImageString = product.ImageString

                    }).ToList();
                _cache.Set(id, list, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(55)));
            }
            return list;
        }
    }
}