using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using NetMarket.Entities;
using NetMarket.ViewModels.MyOrders;

namespace NetMarket.Interfaces
{
    public interface ISearch
    {
        List<OrderViewModel> GetSearchData(object search, IMemoryCache cache, NetMarketDbContext netMarketDbContext);
    }
}