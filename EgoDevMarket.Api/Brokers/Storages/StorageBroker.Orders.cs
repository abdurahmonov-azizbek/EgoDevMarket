// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EgoDevMarket.Api.Models.Orders;
using Microsoft.EntityFrameworkCore;

namespace EgoDevMarket.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Order> Orders { get; set; }

        public async ValueTask<Order> InsertOrderAsync(Order order) =>
            await InsertAsync(order);

        public IQueryable<Order> SelectAllOrders() =>
            SelectAll<Order>();

        public async ValueTask<Order> SelectOrderByIdAsync(Guid orderId) =>
            await SelectAsync<Order>(orderId);

        public async ValueTask<Order> DeleteOrderAsync(Order order) =>
            await DeleteAsync(order);

        public async ValueTask<Order> UpdateOrderAsync(Order order) =>
            await UpdateAsync(order);
    }
}