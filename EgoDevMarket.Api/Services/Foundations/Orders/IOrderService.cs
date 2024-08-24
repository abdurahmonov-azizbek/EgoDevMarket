// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EgoDevMarket.Api.Models.Orders;

namespace EgoDevMarket.Api.Services.Foundations.Orders
{
    public interface IOrderService  
    {
        /// <exception cref="Models.Orders.Exceptions.OrderValidationException"></exception>
        /// <exception cref="Models.Orders.Exceptions.OrderDependencyValidationException"></exception>
        /// <exception cref="Models.Orders.Exceptions.OrderDependencyException"></exception>
        /// <exception cref="Models.Orders.Exceptions.OrderServiceException"></exception>
        ValueTask<Order> AddOrderAsync(Order order);

        /// <exception cref="Models.Orders.Exceptions.OrderDependencyException"></exception>
        /// <exception cref="Models.Orders.Exceptions.OrderServiceException"></exception>
        IQueryable<Order> RetrieveAllOrders();

        /// <exception cref="Models.Orders.Exceptions.OrderDependencyException"></exception>
        /// <exception cref="Models.Orders.Exceptions.OrderServiceException"></exception>
        ValueTask<Order> RetrieveOrderByIdAsync(Guid orderId);

        /// <exception cref="Models.Orders.Exceptions.OrderValidationException"></exception>
        /// <exception cref="Models.Orders.Exceptions.OrderDependencyValidationException"></exception>
        /// <exception cref="Models.Orders.Exceptions.OrderDependencyException"></exception>
        /// <exception cref="Models.Orders.Exceptions.OrderServiceException"></exception>
        ValueTask<Order> ModifyOrderAsync(Order order);

        /// <exception cref="Models.Orders.Exceptions.OrderDependencyValidationException"></exception>
        /// <exception cref="Models.Orders.Exceptions.OrderDependencyException"></exception>
        /// <exception cref="Models.Orders.Exceptions.OrderServiceException"></exception>
        ValueTask<Order> RemoveOrderByIdAsync(Guid orderId);
    }
}