// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System.Linq;
using EgoDevMarket.Api.Models.Orders;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace EgoDevMarket.Api.Tests.Unit.Services.Foundations.Orders
{
    public partial class OrderServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllOrders()
        {
            //given
            IQueryable<Order> randomOrders = CreateRandomOrders();
            IQueryable<Order> storageOrders = randomOrders;
            IQueryable<Order> expectedOrders = storageOrders.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllOrders()).Returns(storageOrders);

            //when
            IQueryable<Order> actualOrders =
                this.orderService.RetrieveAllOrders();

            //then
            actualOrders.Should().BeEquivalentTo(expectedOrders);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllOrders(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
