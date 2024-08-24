// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EgoDevMarket.Api.Brokers.DateTimes;
using EgoDevMarket.Api.Brokers.Loggings;
using EgoDevMarket.Api.Brokers.Storages;
using EgoDevMarket.Api.Models.Orders;

namespace EgoDevMarket.Api.Services.Foundations.Orders
{
    public partial class OrderService : IOrderService
    {

        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public OrderService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)

        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Order> AddOrderAsync(Order order) =>
        TryCatch(async () =>
        {
            ValidateOrderOnAdd(order);

            return await this.storageBroker.InsertOrderAsync(order);
        });

        public IQueryable<Order> RetrieveAllOrders() =>
            TryCatch(() => this.storageBroker.SelectAllOrders());

        public ValueTask<Order> RetrieveOrderByIdAsync(Guid orderId) =>
           TryCatch(async () =>
           {
               ValidateOrderId(orderId);

               Order maybeOrder =
                   await storageBroker.SelectOrderByIdAsync(orderId);

               ValidateStorageOrder(maybeOrder, orderId);

               return maybeOrder;
           });

        public ValueTask<Order> ModifyOrderAsync(Order order) =>
            TryCatch(async () =>
            {
                ValidateOrderOnModify(order);

                Order maybeOrder =
                    await this.storageBroker.SelectOrderByIdAsync(order.Id);

                ValidateAgainstStorageOrderOnModify(inputOrder: order, storageOrder: maybeOrder);

                return await this.storageBroker.UpdateOrderAsync(order);
            });

        public ValueTask<Order> RemoveOrderByIdAsync(Guid orderId) =>
           TryCatch(async () =>
           {
               ValidateOrderId(orderId);

               Order maybeOrder =
                   await this.storageBroker.SelectOrderByIdAsync(orderId);

               ValidateStorageOrder(maybeOrder, orderId);

               return await this.storageBroker.DeleteOrderAsync(maybeOrder);
           });
    }
}