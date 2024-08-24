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
        public async Task ShouldModifyProductAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDateTime();
            Product randomProduct = CreateRandomModifyProduct(randomDate);
            Product inputProduct = randomProduct;
            Product storageProduct = inputProduct.DeepClone();
            storageProduct.UpdatedDate = randomProduct.CreatedDate;
            Product updatedProduct = inputProduct;
            Product expectedProduct = updatedProduct.DeepClone();
            Guid productId = inputProduct.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectProductByIdAsync(productId))
                    .ReturnsAsync(storageProduct);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateProductAsync(inputProduct))
                    .ReturnsAsync(updatedProduct);

            // when
            Product actualProduct =
               await this.productService.ModifyProductAsync(inputProduct);

            // then
            actualProduct.Should().BeEquivalentTo(expectedProduct);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectProductByIdAsync(productId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateProductAsync(inputProduct), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
