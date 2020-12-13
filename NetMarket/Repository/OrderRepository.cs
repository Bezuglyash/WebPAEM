using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using NetMarket.Entities;
using NetMarket.Interfaces;
using NetMarket.Models;
using NetMarket.Search;
using NetMarket.ViewModels.MyOrders;

namespace NetMarket.Repository
{
    /// <summary>
    /// Класс-репозиторий для работы с заказами на уровне доступа к данным
    /// </summary>
    public class OrderRepository
    {
        private NetMarketDbContext _netMarketDbContext;
        private IMemoryCache _cache;
        private ISearch _search;

        public OrderRepository(NetMarketDbContext netMarketDbContext, IMemoryCache cache)
        {
            _netMarketDbContext = netMarketDbContext;
            _cache = cache;
            _search = new SearchByOrderNumber();
        }

        /// <summary>
        /// Метод создания нового заказа
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="time">Время оформления заказа</param>
        /// <param name="name">Имя</param>
        /// <param name="surname">Фамилия</param>
        /// <param name="middleName">Отчество</param>
        /// <param name="email">Email</param>
        /// <param name="phoneNumber">Номер телефона</param>
        /// <param name="address">Адрес доставки</param>
        /// <param name="comment">Комментарий к заказу</param>
        /// <param name="sum">Сумма заказа</param>
        /// <param name="productsId">Список ID продуктов заказа</param>
        /// <returns></returns>
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

        /// <summary>
        /// Метод получения всех заказов
        /// </summary>
        /// <returns>Список заказаов</returns>
        public List<OrderViewModel> GetAllOrders()
        {
            if (!_cache.TryGetValue("allOrders", out List<OrderViewModel> list))
            {
                list = (from order in _netMarketDbContext.Orders
                    orderby order.Id descending
                    select new OrderViewModel
                    {
                        OrderNumber = order.Id,
                        CustomerFullName = order.MiddleName != null ? order.Surname + " " + order.Name + " " + order.MiddleName : order.Surname + " " + order.Name,
                        Address = order.Address,
                        OrderDate = order.OrderTime.ToShortDateString() + " в " + order.OrderTime.ToShortTimeString(),
                        Sum = order.Sum,
                        Status = (from st in _netMarketDbContext.OrdersStatus
                            join ord in _netMarketDbContext.Orders on st.Id equals ord.OrderStatusId
                            where st.Id == order.OrderStatusId
                            select st.Status).ToList()[0],
                        Comment = order.Comment ?? "-",
                        Email = order.Email,
                        PhoneNumber = order.PhoneNumber,
                        InformationForEmployee = order.UserId != null ? "Зарегистрированный пользователь" : "Связаться можно только через телефон или Email"
                    }).ToList();
                _cache.Set("allOrders", list, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(55)));
            }
            return list;
        }

        /// <summary>
        /// Метод получения всех заказов с учётом поиска
        /// </summary>
        /// <param name="search">Поиск</param>
        /// <returns>Список заказов</returns>
        public List<OrderViewModel> GetSearchOrders(string search)
        {
            if (int.TryParse(search, out int searchInt))
            {
                return _search.GetSearchData(searchInt, _cache, _netMarketDbContext);
            }
            _search = new SearchByName();
            return _search.GetSearchData(search, _cache, _netMarketDbContext);
        }

        /// <summary>
        /// Метод получения статуса заказа
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <returns>Статус заказа</returns>
        public string GetOrderStatus(int id)
        {
            if (_cache.TryGetValue("allOrders", out List<OrderViewModel> list))
            {
                return (from order in list
                    where order.OrderNumber == id
                    select order.Status).ToList()[0];
            }
            return (from order in _netMarketDbContext.Orders
                where order.Id == id
                select order.OrderStatus.Status).ToList()[0];
        }

        /// <summary>
        /// Метод получения истории заказов клиента
        /// </summary>
        /// <param name="login">Логин клиента</param>
        /// <returns>Список заказаов</returns>
        public List<OrderViewModel> GetAllUserOrders(string login)
        {
            return (from order in _netMarketDbContext.Orders
                where order.User.Login == login
                orderby order.Id descending
                select new OrderViewModel
                {
                    OrderNumber = order.Id,
                    CustomerFullName = order.MiddleName != null ? order.Surname + " " + order.Name + " " + order.MiddleName : order.Surname + " " + order.Name,
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

        /// <summary>
        /// Метод получения истории заказаов клиента с учётом поиска
        /// </summary>
        /// <param name="login">Логин клиента</param>
        /// <param name="search">Поиск</param>
        /// <returns>Список заказаов</returns>
        public List<OrderViewModel> GetSearchUserOrders(string login, string search)
        {
            if (int.TryParse(search, out int searchInt))
            {
                return (from order in _netMarketDbContext.Orders
                    where order.User.Login == login && order.Id == searchInt
                    orderby order.Id descending
                    select new OrderViewModel
                    {
                        OrderNumber = order.Id,
                        CustomerFullName = order.MiddleName != null ? order.Surname + " " + order.Name + " " + order.MiddleName : order.Surname + " " + order.Name,
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
            return (from order in _netMarketDbContext.Orders
                where order.User.Login == login && (order.Name.Contains(search) || order.Surname.Contains(search) || order.MiddleName != null && order.MiddleName.Contains(search))
                orderby order.Id descending
                select new OrderViewModel
                {
                    OrderNumber = order.Id,
                    CustomerFullName = order.MiddleName != null ? order.Surname + " " + order.Name + " " + order.MiddleName : order.Surname + " " + order.Name,
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

        /// <summary>
        /// Метод получения списка товаров в заказе
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <returns>Список товаров</returns>
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
                        ImageString = product.ImageString,
                        HaveInStock = product.HaveInStock

                    }).ToList();
                _cache.Set(id, list, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(28)));
            }
            return list;
        }

        /// <summary>
        /// Метод удаления из кеша товаров, которые удалены из интернет-магазина
        /// </summary>
        /// <param name="productId">ID товара</param>
        public void DeleteAllProductsThatAreOutOfStock(int productId)
        {
            var list = (from order in _netMarketDbContext.Orders
                join orderProduct in _netMarketDbContext.OrderProducts on order.Id equals orderProduct.OrderId
                join product in _netMarketDbContext.Products on orderProduct.ProductId equals product.Id
                where product.Id == productId
                select order.Id).ToList();
            foreach (var id in list)
            {
                _cache.Remove(id);
            }
        }

        /// <summary>
        /// Метод обновления статуса заказа
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <param name="newStatusId">ID нового статуса</param>
        /// <returns></returns>
        public async Task OrderStatusUpdateAsync(int id, int newStatusId)
        {
            _cache.Remove("allOrders");
            var order = _netMarketDbContext.Orders.Find(id);
            order.OrderStatusId = newStatusId;
            _netMarketDbContext.Orders.Update(order);
            await _netMarketDbContext.SaveChangesAsync();
        }
    }
}