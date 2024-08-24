// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EgoDevMarket.Api.Models.Orders;

namespace EgoDevMarket.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Order> InsertOrderAsync(Order order);
        IQueryable<Order> SelectAllOrders();
        ValueTask<Order> SelectOrderByIdAsync(Guid orderId);
        ValueTask<Order> DeleteOrderAsync(Order order);
        ValueTask<Order> UpdateOrderAsync(Order order);
    }
}