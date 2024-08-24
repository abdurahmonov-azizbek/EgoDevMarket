// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using EgoDevMarket.Api.Models.Orders;
using EgoDevMarket.Api.Models.Orders.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EgoDevMarket.Api.Tests.Unit.Services.Foundations.Orders
{
    public partial class OrderServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someOrderId = Guid.NewGuid();

            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedOrderException =
                new LockedOrderException(databaseUpdateConcurrencyException);

            var expectedOrderDependencyValidationException =
                new OrderDependencyValidationException(lockedOrderException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOrderByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<Order> removeOrderByIdTask =
               this.orderService.RemoveOrderByIdAsync(someOrderId);

            OrderDependencyValidationException actualOrderDependencyValidationException =
                await Assert.ThrowsAsync<OrderDependencyValidationException>(
                    removeOrderByIdTask.AsTask);

            // then
            actualOrderDependencyValidationException.Should().BeEquivalentTo(
               expectedOrderDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOrderByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOrderDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteOrderAsync(It.IsAny<Order>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveWhenSqlExceptionOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedOrderStorageException =
                new FailedOrderStorageException(sqlException);

            var expectedOrderDependencyException =
                new OrderDependencyException(failedOrderStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOrderByIdAsync(someId))
                    .Throws(sqlException);
            // when
            ValueTask<Order> removeOrderTask =
                this.orderService.RemoveOrderByIdAsync(someId);

            OrderDependencyException actualOrderDependencyException =
                await Assert.ThrowsAsync<OrderDependencyException>(
                    removeOrderTask.AsTask);

            // then
            actualOrderDependencyException.Should().BeEquivalentTo(expectedOrderDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOrderByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedOrderDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someOrderId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedOrderServiceException =
                new FailedOrderServiceException(serviceException);

            var expectedOrderServiceException =
                new OrderServiceException(failedOrderServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOrderByIdAsync(someOrderId))
                    .Throws(serviceException);

            // when
            ValueTask<Order> removeOrderByIdTask =
                this.orderService.RemoveOrderByIdAsync(someOrderId);

            OrderServiceException actualOrderServiceException =
                await Assert.ThrowsAsync<OrderServiceException>(
                    removeOrderByIdTask.AsTask);

            // then
            actualOrderServiceException.Should().BeEquivalentTo(expectedOrderServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOrderByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOrderServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}