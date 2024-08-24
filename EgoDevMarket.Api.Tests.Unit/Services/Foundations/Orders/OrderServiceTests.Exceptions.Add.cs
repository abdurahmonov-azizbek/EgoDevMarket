// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using EgoDevMarket.Api.Models.Orders;
using EgoDevMarket.Api.Models.Orders.Exceptions;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            Order someOrder = CreateRandomOrder();
            Guid orderId = someOrder.Id;
            SqlException sqlException = CreateSqlException();

            FailedOrderStorageException failedOrderStorageException =
                new FailedOrderStorageException(sqlException);

            OrderDependencyException expectedOrderDependencyException =
                new OrderDependencyException(failedOrderStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<Order> addOrderTask = this.orderService
                .AddOrderAsync(someOrder);

            OrderDependencyException actualOrderDependencyException =
                await Assert.ThrowsAsync<OrderDependencyException>(addOrderTask.AsTask);

            // then
            actualOrderDependencyException.Should().BeEquivalentTo(expectedOrderDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(expectedOrderDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorOccurredAndLogItAsync()
        {
            // given
            Order someOrder = CreateRandomOrder();
            string someMessage = GetRandomString();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsOrderException =
                new AlreadyExistsOrderException(duplicateKeyException);

            var expectedOrderDependencyValidationException =
                new OrderDependencyValidationException(alreadyExistsOrderException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(duplicateKeyException);

            // when
            ValueTask<Order> addOrderTask = this.orderService.AddOrderAsync(someOrder);

            OrderDependencyValidationException actualOrderDependencyValidationException =
                await Assert.ThrowsAsync<OrderDependencyValidationException>(
                    addOrderTask.AsTask);

            // then
            actualOrderDependencyValidationException.Should().BeEquivalentTo(
                expectedOrderDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOrderDependencyValidationException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateErrorOccursAndLogItAsync()
        {
            // given
            Order someOrder = CreateRandomOrder();
            var dbUpdateException = new DbUpdateException();

            var failedOrderStorageException = new FailedOrderStorageException(dbUpdateException);

            var expectedOrderDependencyException =
                new OrderDependencyException(failedOrderStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(dbUpdateException);

            // when
            ValueTask<Order> addOrderTask = this.orderService.AddOrderAsync(someOrder);

            OrderDependencyException actualOrderDependencyException =
                 await Assert.ThrowsAsync<OrderDependencyException>(addOrderTask.AsTask);

            // then
            actualOrderDependencyException.Should()
                .BeEquivalentTo(expectedOrderDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedOrderDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateOrderAsync(It.IsAny<Order>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccuredAndLogItAsync()
        {
            // given
            Order someOrder = CreateRandomOrder();
            string someMessage = GetRandomString();

            var dbUpdateException =
                new DbUpdateException(someMessage);

            var failedOrderStorageException =
                new FailedOrderStorageException(dbUpdateException);

            var expectedOrderDependencyException =
                new OrderDependencyException(failedOrderStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetCurrentDateTimeOffset()).Throws(dbUpdateException);

            // when
            ValueTask<Order> addOrderTask =
                this.orderService.AddOrderAsync(someOrder);

            OrderDependencyException actualOrderDependencyException =
                await Assert.ThrowsAsync<OrderDependencyException>(addOrderTask.AsTask);

            // then
            actualOrderDependencyException.Should().BeEquivalentTo(expectedOrderDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOrderDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccuredAndLogItAsync()
        {
            //given
            Order someOrder = CreateRandomOrder();
            var serviceException = new Exception();
            var failedOrderException = new FailedOrderServiceException(serviceException);

            var expectedOrderServiceExceptions =
                new OrderServiceException(failedOrderException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(serviceException);

            //when
            ValueTask<Order> addOrderTask = this.orderService.AddOrderAsync(someOrder);

            OrderServiceException actualOrderServiceException =
                await Assert.ThrowsAsync<OrderServiceException>(addOrderTask.AsTask);

            //then
            actualOrderServiceException.Should().BeEquivalentTo(
                expectedOrderServiceExceptions);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOrderServiceExceptions))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}