// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using EgoDevMarket.Api.Models.Products;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace EgoDevMarket.Api.Tests.Unit.Services.Foundations.Products
{
    public partial class ProductServiceTests
    {
        [Fact]
        public async Task ShouldAddProductAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            Product randomProduct = CreateRandomProduct(randomDate);
            Product inputProduct = randomProduct;
            Product persistedProduct = inputProduct;
            Product expectedProduct = persistedProduct.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertProductAsync(inputProduct)).ReturnsAsync(persistedProduct);

            // when
            Product actualProduct = await this.productService
                .AddProductAsync(inputProduct);

            // then
            actualProduct.Should().BeEquivalentTo(expectedProduct);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertProductAsync(inputProduct), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}