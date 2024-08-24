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
        public async Task ShouldModifyOrderAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDateTime();
            Order randomOrder = CreateRandomModifyOrder(randomDate);
            Order inputOrder = randomOrder;
            Order storageOrder = inputOrder.DeepClone();
            storageOrder.UpdatedDate = randomOrder.CreatedDate;
            Order updatedOrder = inputOrder;
            Order expectedOrder = updatedOrder.DeepClone();
            Guid orderId = inputOrder.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOrderByIdAsync(orderId))
                    .ReturnsAsync(storageOrder);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateOrderAsync(inputOrder))
                    .ReturnsAsync(updatedOrder);

            // when
            Order actualOrder =
               await this.orderService.ModifyOrderAsync(inputOrder);

            // then
            actualOrder.Should().BeEquivalentTo(expectedOrder);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOrderByIdAsync(orderId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateOrderAsync(inputOrder), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
