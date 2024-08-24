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
        public async Task ShouldAddOrderAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            Order randomOrder = CreateRandomOrder(randomDate);
            Order inputOrder = randomOrder;
            Order persistedOrder = inputOrder;
            Order expectedOrder = persistedOrder.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertOrderAsync(inputOrder)).ReturnsAsync(persistedOrder);

            // when
            Order actualOrder = await this.orderService
                .AddOrderAsync(inputOrder);

            // then
            actualOrder.Should().BeEquivalentTo(expectedOrder);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertOrderAsync(inputOrder), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}