// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using EgoDevMarket.Api.Models.Orders.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

namespace EgoDevMarket.Api.Tests.Unit.Services.Foundations.Orders
{
    public partial class OrderServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            //given
            SqlException sqlException = CreateSqlException();

            var failedStorageException =
                new FailedOrderStorageException(sqlException);

            var expectedOrderDependencyException =
                new OrderDependencyException(failedStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllOrders()).Throws(sqlException);

            //when
            Action retrieveAllOrdersAction = () =>
                this.orderService.RetrieveAllOrders();

            OrderDependencyException actualOrderDependencyException =
                Assert.Throws<OrderDependencyException>(retrieveAllOrdersAction);

            //then
            actualOrderDependencyException.Should().BeEquivalentTo(
                expectedOrderDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllOrders(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedOrderDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomString();
            var serviceException = new Exception(exceptionMessage);

            var failedOrderServiceException =
                new FailedOrderServiceException(serviceException);

            var expectedOrderServiceException =
                new OrderServiceException(failedOrderServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllOrders()).Throws(serviceException);

            // when
            Action retrieveAllOrdersAction = () =>
                this.orderService.RetrieveAllOrders();

            OrderServiceException actualOrderServiceException =
                Assert.Throws<OrderServiceException>(retrieveAllOrdersAction);

            // then
            actualOrderServiceException.Should().BeEquivalentTo(expectedOrderServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllOrders(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedOrderServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}