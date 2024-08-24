// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using EgoDevMarket.Api.Models.Products;
using EgoDevMarket.Api.Models.Products.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace EgoDevMarket.Api.Tests.Unit.Services.Foundations.Products
{
    public partial class ProductServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            //given
            var invalidProductId = Guid.Empty;
            var invalidProductException = new InvalidProductException();

            invalidProductException.AddData(
                key: nameof(Product.Id),
                values: "Id is required");

            var excpectedProductValidationException = new
                ProductValidationException(invalidProductException);

            //when
            ValueTask<Product> retrieveProductByIdTask =
                this.productService.RetrieveProductByIdAsync(invalidProductId);

            ProductValidationException actuallProductValidationException =
                await Assert.ThrowsAsync<ProductValidationException>(
                    retrieveProductByIdTask.AsTask);

            //then
            actuallProductValidationException.Should()
                .BeEquivalentTo(excpectedProductValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    excpectedProductValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectProductByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfProductIsNotFoundAndLogItAsync()
        {
            Guid someProductId = Guid.NewGuid();
            Product noProduct = null;

            var notFoundProductException =
                new NotFoundProductException(someProductId);

            var excpectedProductValidationException =
                new ProductValidationException(notFoundProductException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectProductByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noProduct);

            //when 
            ValueTask<Product> retrieveProductByIdTask =
                this.productService.RetrieveProductByIdAsync(someProductId);

            ProductValidationException actualProductValidationException =
                await Assert.ThrowsAsync<ProductValidationException>(
                    retrieveProductByIdTask.AsTask);

            //then
            actualProductValidationException.Should()
                .BeEquivalentTo(excpectedProductValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectProductByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    excpectedProductValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
