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
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            // given
            Product nullProduct = null;
            var nullProductException = new NullProductException();

            var expectedProductValidationException =
                new ProductValidationException(nullProductException);

            // when
            ValueTask<Product> addProductTask = this.productService.AddProductAsync(nullProduct);

            ProductValidationException actualProductValidationException =
                await Assert.ThrowsAsync<ProductValidationException>(addProductTask.AsTask);

            // then
            actualProductValidationException.Should()
                .BeEquivalentTo(expectedProductValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(
                    SameExceptionAs(expectedProductValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertProductAsync(It.IsAny<Product>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfJobIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            Product invalidProduct = new Product()
            {
                Name = invalidText
            };

            var invalidProductException = new InvalidProductException();

				invalidProductException.AddData(
					key: nameof(Product.Id),
					values: "Id is required");

				invalidProductException.AddData(
					key: nameof(Product.Name),
					values: "Text is required");

				invalidProductException.AddData(
					key: nameof(Product.Description{get;set;}),
					values: "Text is required");

				invalidProductException.AddData(
					key: nameof(Product.CreatedDate),
					values: "Date is required");

				invalidProductException.AddData(
					key: nameof(Product.UpdatedDate),
					values: "Date is required");



            var expectedProductValidationException =
                new ProductValidationException(invalidProductException);

            // when
            ValueTask<Product> addProductTask = this.productService.AddProductAsync(invalidProduct);

            ProductValidationException actualProductValidationException =
                await Assert.ThrowsAsync<ProductValidationException>(addProductTask.AsTask);

            // then
            actualProductValidationException.Should()
                .BeEquivalentTo(expectedProductValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedProductValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertProductAsync(It.IsAny<Product>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShoudlThrowValidationExceptionOnAddIfCreatedDateIsNotSameAsUpdatedDateAndLogItAsync()
        {
            // given
            int randomMinutes = GetRandomNumber();
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            Product randomProduct = CreateRandomProduct(randomDate);
            Product invalidProduct = randomProduct;
            invalidProduct.UpdatedDate = randomDate.AddMinutes(randomMinutes);
            var invalidProductException = new InvalidProductException();

            invalidProductException.AddData(
                key: nameof(Product.CreatedDate),
                values: $"Date is not same as {nameof(Product.UpdatedDate)}");

            var expectedProductValidationException = new ProductValidationException(invalidProductException);

            this.dateTimeBrokerMock.Setup(broker => broker.GetCurrentDateTimeOffset())
                .Returns(randomDate);

            // when
            ValueTask<Product> addProductTask = this.productService.AddProductAsync(invalidProduct);

            ProductValidationException actualProductValidationException =
                await Assert.ThrowsAsync<ProductValidationException>(addProductTask.AsTask);

            // then
            actualProductValidationException.Should().BeEquivalentTo(expectedProductValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedProductValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertProductAsync(It.IsAny<Product>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidMinutes))]
        public async Task ShouldThrowValidationExceptionIfCreatedDateIsNotRecentAndLogItAsync(
            int invalidMinutes)
        {
            // given
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            DateTimeOffset invalidDateTime = randomDate.AddMinutes(invalidMinutes);
            Product randomProduct = CreateRandomProduct(invalidDateTime);
            Product invalidProduct = randomProduct;
            var invalidProductException = new InvalidProductException();

            invalidProductException.AddData(
                key: nameof(Product.CreatedDate),
                values: "Date is not recent");

            var expectedProductValidationException =
                new ProductValidationException(invalidProductException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            // when
            ValueTask<Product> addProductTask = this.productService.AddProductAsync(invalidProduct);

            ProductValidationException actualProductValidationException =
                await Assert.ThrowsAsync<ProductValidationException>(addProductTask.AsTask);

            // then
            actualProductValidationException.Should().
                BeEquivalentTo(expectedProductValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
            broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedProductValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertProductAsync(It.IsAny<Product>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}