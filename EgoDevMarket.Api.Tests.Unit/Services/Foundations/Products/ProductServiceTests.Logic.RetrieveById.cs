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
        public async Task ShouldRetrieveProductByIdAsync()
        {
            //given
            Guid randomProductId = Guid.NewGuid();
            Guid inputProductId = randomProductId;
            Product randomProduct = CreateRandomProduct();
            Product storageProduct = randomProduct;
            Product excpectedProduct = randomProduct.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectProductByIdAsync(inputProductId)).ReturnsAsync(storageProduct);

            //when
            Product actuallProduct = await this.productService.RetrieveProductByIdAsync(inputProductId);

            //then
            actuallProduct.Should().BeEquivalentTo(excpectedProduct);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectProductByIdAsync(inputProductId), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}