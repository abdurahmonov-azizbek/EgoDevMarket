// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
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
        public async Task ShouldRemoveOrderByIdAsync()
        {
            // given
            Guid randomOrderId = Guid.NewGuid();
            Guid inputOrderId = randomOrderId;
            Order randomOrder = CreateRandomOrder();
            Order storageOrder = randomOrder;
            Order expectedInputOrder = storageOrder;
            Order deletedOrder = expectedInputOrder;
            Order expectedOrder = deletedOrder.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOrderByIdAsync(inputOrderId))
                    .ReturnsAsync(storageOrder);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteOrderAsync(expectedInputOrder))
                    .ReturnsAsync(deletedOrder);

            // when
            Order actualOrder = await this.orderService
                .RemoveOrderByIdAsync(inputOrderId);

            // then
            actualOrder.Should().BeEquivalentTo(expectedOrder);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOrderByIdAsync(inputOrderId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteOrderAsync(expectedInputOrder), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
