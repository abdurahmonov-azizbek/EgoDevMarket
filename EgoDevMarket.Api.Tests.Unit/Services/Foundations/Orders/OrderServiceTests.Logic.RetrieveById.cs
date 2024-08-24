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
        public async Task ShouldRetrieveOrderByIdAsync()
        {
            //given
            Guid randomOrderId = Guid.NewGuid();
            Guid inputOrderId = randomOrderId;
            Order randomOrder = CreateRandomOrder();
            Order storageOrder = randomOrder;
            Order excpectedOrder = randomOrder.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectOrderByIdAsync(inputOrderId)).ReturnsAsync(storageOrder);

            //when
            Order actuallOrder = await this.orderService.RetrieveOrderByIdAsync(inputOrderId);

            //then
            actuallOrder.Should().BeEquivalentTo(excpectedOrder);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectOrderByIdAsync(inputOrderId), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}