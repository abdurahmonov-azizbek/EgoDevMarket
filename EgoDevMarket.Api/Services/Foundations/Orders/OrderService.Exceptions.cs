// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EgoDevMarket.Api.Models.Orders;
using EgoDevMarket.Api.Models.Orders.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace EgoDevMarket.Api.Services.Foundations.Orders
{
    public partial class OrderService
    {
        private delegate ValueTask<Order> ReturningOrderFunction();
        private delegate IQueryable<Order> ReturningOrdersFunction();

        private async ValueTask<Order> TryCatch(ReturningOrderFunction returningOrderFunction)
        {
            try
            {
                return await returningOrderFunction();
            }
            catch (NullOrderException nullOrderException)
            {
                throw CreateAndLogValidationException(nullOrderException);
            }
            catch (InvalidOrderException invalidOrderException)
            {
                throw CreateAndLogValidationException(invalidOrderException);
            }
            catch (NotFoundOrderException notFoundOrderException)
            {
                throw CreateAndLogValidationException(notFoundOrderException);
            }
            catch (SqlException sqlException)
            {
                var failedOrderStorageException = new FailedOrderStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedOrderStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsOrderException = new AlreadyExistsOrderException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsOrderException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedOrderException = new LockedOrderException(dbUpdateConcurrencyException);

                throw CreateAndLogDependencyValidationException(lockedOrderException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedOrderStorageException = new FailedOrderStorageException(databaseUpdateException);

                throw CreateAndLogDependencyException(failedOrderStorageException);
            }
            catch (Exception exception)
            {
                var failedOrderServiceException = new FailedOrderServiceException(exception);

                throw CreateAndLogServiceException(failedOrderServiceException);
            }
        }

        private IQueryable<Order> TryCatch(ReturningOrdersFunction returningOrdersFunction)
        {
            try
            {
                return returningOrdersFunction();
            }
            catch (SqlException sqlException)
            {
                var failedOrderStorageException = new FailedOrderStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedOrderStorageException);
            }
            catch (Exception serviceException)
            {
                var failedOrderServiceException = new FailedOrderServiceException(serviceException);

                throw CreateAndLogServiceException(failedOrderServiceException);
            }
        }

        private OrderValidationException CreateAndLogValidationException(Xeption exception)
        {
            var orderValidationException = new OrderValidationException(exception);
            this.loggingBroker.LogError(orderValidationException);

            return orderValidationException;
        }

        private OrderDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var OrderDependencyException = new OrderDependencyException(exception);
            this.loggingBroker.LogCritical(OrderDependencyException);

            return OrderDependencyException;
        }

        private OrderDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var orderDependencyException = new OrderDependencyException(exception);
            this.loggingBroker.LogError(orderDependencyException);

            return orderDependencyException;
        }


        private OrderDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var orderDependencyValidationException = new OrderDependencyValidationException(exception);
            this.loggingBroker.LogError(orderDependencyValidationException);

            return orderDependencyValidationException;
        }

        private OrderServiceException CreateAndLogServiceException(Xeption innerException)
        {
            var orderServiceException = new OrderServiceException(innerException);
            this.loggingBroker.LogError(orderServiceException);

            return orderServiceException;
        }
    }
}