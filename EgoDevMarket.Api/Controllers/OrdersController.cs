// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EgoDevMarket.Api.Models.Orders;
using EgoDevMarket.Api.Models.Orders.Exceptions;
using EgoDevMarket.Api.Services.Foundations.Orders;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace EgoDevMarket.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : RESTFulController
    {
        private readonly IOrderService orderService;

        public OrdersController(IOrderService orderService) =>
            this.orderService = orderService;

        [HttpPost]
        public async ValueTask<ActionResult<Order>> PostOrderAsync(Order order)
        {
            try
            {
                Order addedOrder = await this.orderService.AddOrderAsync(order);

                return Created(addedOrder);
            }
            catch (OrderValidationException orderValidationException)
            {
                return BadRequest(orderValidationException.InnerException);
            }
            catch (OrderDependencyValidationException orderDependencyValidationException)
                when (orderDependencyValidationException.InnerException is AlreadyExistsOrderException)
            {
                return Conflict(orderDependencyValidationException.InnerException);
            }
            catch (OrderDependencyException orderDependencyException)
            {
                return InternalServerError(orderDependencyException.InnerException);
            }
            catch (OrderServiceException orderServiceException)
            {
                return InternalServerError(orderServiceException.InnerException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<Order>> GetAllOrders()
        {
            try
            {
                IQueryable<Order> allOrders = this.orderService.RetrieveAllOrders();

                return Ok(allOrders);
            }
            catch (OrderDependencyException orderDependencyException)
            {
                return InternalServerError(orderDependencyException.InnerException);
            }
            catch (OrderServiceException orderServiceException)
            {
                return InternalServerError(orderServiceException.InnerException);
            }
        }

        [HttpGet("{orderId}")]
        public async ValueTask<ActionResult<Order>> GetOrderByIdAsync(Guid orderId)
        {
            try
            {
                return await this.orderService.RetrieveOrderByIdAsync(orderId);
            }
            catch (OrderDependencyException orderDependencyException)
            {
                return InternalServerError(orderDependencyException);
            }
            catch (OrderValidationException orderValidationException)
                when (orderValidationException.InnerException is InvalidOrderException)
            {
                return BadRequest(orderValidationException.InnerException);
            }
            catch (OrderValidationException orderValidationException)
                 when (orderValidationException.InnerException is NotFoundOrderException)
            {
                return NotFound(orderValidationException.InnerException);
            }
            catch (OrderServiceException orderServiceException)
            {
                return InternalServerError(orderServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<Order>> PutOrderAsync(Order order)
        {
            try
            {
                Order modifiedOrder =
                    await this.orderService.ModifyOrderAsync(order);

                return Ok(modifiedOrder);
            }
            catch (OrderValidationException orderValidationException)
                when (orderValidationException.InnerException is NotFoundOrderException)
            {
                return NotFound(orderValidationException.InnerException);
            }
            catch (OrderValidationException orderValidationException)
            {
                return BadRequest(orderValidationException.InnerException);
            }
            catch (OrderDependencyValidationException orderDependencyValidationException)
            {
                return BadRequest(orderDependencyValidationException.InnerException);
            }
            catch (OrderDependencyException orderDependencyException)
            {
                return InternalServerError(orderDependencyException.InnerException);
            }
            catch (OrderServiceException orderServiceException)
            {
                return InternalServerError(orderServiceException.InnerException);
            }
        }

        [HttpDelete("{orderId}")]
        public async ValueTask<ActionResult<Order>> DeleteOrderByIdAsync(Guid orderId)
        {
            try
            {
                Order deletedOrder = await this.orderService.RemoveOrderByIdAsync(orderId);

                return Ok(deletedOrder);
            }
            catch (OrderValidationException orderValidationException)
                when (orderValidationException.InnerException is NotFoundOrderException)
            {
                return NotFound(orderValidationException.InnerException);
            }
            catch (OrderValidationException orderValidationException)
            {
                return BadRequest(orderValidationException.InnerException);
            }
            catch (OrderDependencyValidationException orderDependencyValidationException)
                when (orderDependencyValidationException.InnerException is LockedOrderException)
            {
                return Locked(orderDependencyValidationException.InnerException);
            }
            catch (OrderDependencyValidationException orderDependencyValidationException)
            {
                return BadRequest(orderDependencyValidationException.InnerException);
            }
            catch (OrderDependencyException orderDependencyException)
            {
                return InternalServerError(orderDependencyException.InnerException);
            }
            catch (OrderServiceException orderServiceException)
            {
                return InternalServerError(orderServiceException.InnerException);
            }
        }
    }
}