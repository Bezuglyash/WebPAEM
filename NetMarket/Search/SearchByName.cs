using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using NetMarket.Entities;
using NetMarket.Interfaces;
using NetMarket.ViewModels.MyOrders;

namespace NetMarket.Search
{
    public class SearchByName : ISearch
    {
        public List<OrderViewModel> GetSearchData(object search, IMemoryCache cache, NetMarketDbContext netMarketDbContext)
        {
            if (cache.TryGetValue("allOrders", out List<OrderViewModel> list))
            {
                return (from order in list
                    where order.CustomerFullName.Contains((string)search)
                    select order).ToList();
            }
            return (from order in netMarketDbContext.Orders
                where order.Name.Contains((string)search) || order.Surname.Contains((string)search) || order.MiddleName != null && order.MiddleName.Contains((string)search)
                orderby order.Id descending
                select new OrderViewModel
                {
                    OrderNumber = order.Id,
                    CustomerFullName = order.MiddleName != null ? order.Surname + " " + order.Name + " " + order.MiddleName : order.Surname + " " + order.Name,
                    Address = order.Address,
                    OrderDate = order.OrderTime.ToShortDateString() + " в " + order.OrderTime.ToShortTimeString(),
                    Sum = order.Sum,
                    Status = (from st in netMarketDbContext.OrdersStatus
                        join ord in netMarketDbContext.Orders on st.Id equals ord.OrderStatusId
                        where st.Id == order.OrderStatusId
                        select st.Status).ToList()[0],
                    Comment = order.Comment ?? "-",
                    Email = order.Email,
                    PhoneNumber = order.PhoneNumber,
                    InformationForEmployee = order.UserId != null ? "Зарегистрированный пользователь" : "Связаться можно только через телефон или Email"
                }).ToList();
        }
    }
}