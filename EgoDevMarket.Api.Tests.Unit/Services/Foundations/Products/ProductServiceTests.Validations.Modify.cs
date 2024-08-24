// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using EgoDevMarket.Api.Models.Products;
using EgoDevMarket.Api.Models.Products.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace EgoDevMarket.Api.Tests.Unit.Services.Foundations.Products
{
    public partial class ProductServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfProductIsNullAndLogItAsync()
        {
            // given
            Product nullProduct = null;
            var nullProductException = new NullProductException();

            var expectedProductValidationException =
                new ProductValidationException(nullProductException);

            // when
            ValueTask<Product> modifyProductTask = this.productService.ModifyProductAsync(nullProduct);

            ProductValidationException actualProductValidationException =
                await Assert.ThrowsAsync<ProductValidationException>(
                    modifyProductTask.AsTask);

            // then
            actualProductValidationException.Should()
                .BeEquivalentTo(expectedProductValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedProductValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfProductIsInvalidAndLogItAsync(string invalidString)
        {
            // given
            Product invalidProduct = new Product
            {
                Name = invalidString
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
                values: new[]
                    {
                        "Date is required",
                        "Date is not recent",
                        $"Date is the same as {nameof(Product.CreatedDate)}"
                    }
                );

            var expectedProductValidationException =
                new ProductValidationException(invalidProductException);


            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(GetRandomDateTime);

            // when
            ValueTask<Product> modifyProductTask = this.productService.ModifyProductAsync(invalidProduct);

            ProductValidationException actualProductValidationException =
                await Assert.ThrowsAsync<ProductValidationException>(
                    modifyProductTask.AsTask);

            // then
            actualProductValidationException.Should()
                .BeEquivalentTo(expectedProductValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedProductValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Product randomProduct = CreateRandomProduct(randomDateTime);
            Product invalidProduct = randomProduct;
            var invalidProductException = new InvalidProductException();

            invalidProductException.AddData(
                key: nameof(Product.UpdatedDate),
                values: $"Date is the same as {nameof(Product.CreatedDate)}");

            var expectedProductValidationException =
                new ProductValidationException(invalidProductException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDateTime);

            // when
            ValueTask<Product> modifyProductTask =
                this.productService.ModifyProductAsync(invalidProduct);

            ProductValidationException actualProductValidationException =
                 await Assert.ThrowsAsync<ProductValidationException>(
                    modifyProductTask.AsTask);

            // then
            actualProductValidationException.Should()
                .BeEquivalentTo(expectedProductValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectProductByIdAsync(invalidProduct.Id), Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedProductValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidSeconds))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(int minutes)
        {
            // given
            DateTimeOffset dateTime = GetRandomDateTime();
            Product randomProduct = CreateRandomProduct(dateTime);
            Product inputProduct = randomProduct;
            inputProduct.UpdatedDate = dateTime.AddMinutes(minutes);
            var invalidProductException = new InvalidProductException();

            invalidProductException.AddData(
                key: nameof(Product.UpdatedDate),
                values: "Date is not recent");

            var expectedProductValidatonException =
                new ProductValidationException(invalidProductException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(dateTime);

            // when
            ValueTask<Product> modifyProductTask =
                this.productService.ModifyProductAsync(inputProduct);

            ProductValidationException actualProductValidationException =
                await Assert.ThrowsAsync<ProductValidationException>(
                    modifyProductTask.AsTask);

            // then
            actualProductValidationException.Should()
                .BeEquivalentTo(expectedProductValidatonException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedProductValidatonException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectProductByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfProductDoesNotExistAndLogItAsync()
        {
            // given
            int negativeMinutes = GetRandomNegativeNumber();
            DateTimeOffset dateTime = GetRandomDateTime();
            Product randomProduct = CreateRandomProduct(dateTime);
            Product nonExistProduct = randomProduct;
            nonExistProduct.CreatedDate = dateTime.AddMinutes(negativeMinutes);
            Product nullProduct = null;

            var notFoundProductException = new NotFoundProductException(nonExistProduct.Id);

            var expectedProductValidationException =
                new ProductValidationException(notFoundProductException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectProductByIdAsync(nonExistProduct.Id))
                    .ReturnsAsync(nullProduct);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(dateTime);

            // when
            ValueTask<Product> modifyProductTask =
                this.productService.ModifyProductAsync(nonExistProduct);

            ProductValidationException actualProductValidationException =
                await Assert.ThrowsAsync<ProductValidationException>(
                    modifyProductTask.AsTask);

            // then
            actualProductValidationException.Should()
                .BeEquivalentTo(expectedProductValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectProductByIdAsync(nonExistProduct.Id), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedProductValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNegativeNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Product randomProduct = CreateRandomModifyProduct(randomDateTime);
            Product invalidProduct = randomProduct.DeepClone();
            Product storageProduct = invalidProduct.DeepClone();
            storageProduct.CreatedDate = storageProduct.CreatedDate.AddMinutes(randomMinutes);
            storageProduct.UpdatedDate = storageProduct.UpdatedDate.AddMinutes(randomMinutes);
            var invalidProductException = new InvalidProductException();
            Guid productId = invalidProduct.Id;

            invalidProductException.AddData(
                key: nameof(Product.CreatedDate),
                values: $"Date is not same as {nameof(Product.CreatedDate)}");

            var expectedProductValidationException =
                new ProductValidationException(invalidProductException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectProductByIdAsync(productId)).ReturnsAsync(storageProduct);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDateTime);

            // when
            ValueTask<Product> modifyProductTask =
                this.productService.ModifyProductAsync(invalidProduct);

            ProductValidationException actualProductValidationException =
                await Assert.ThrowsAsync<ProductValidationException>(modifyProductTask.AsTask);

            // then
            actualProductValidationException.Should()
                .BeEquivalentTo(expectedProductValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectProductByIdAsync(invalidProduct.Id), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedProductValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Product randomProduct = CreateRandomModifyProduct(randomDateTime);
            Product invalidProduct = randomProduct;
            Product storageProduct = randomProduct.DeepClone();
            invalidProduct.UpdatedDate = storageProduct.UpdatedDate;
            Guid productId = invalidProduct.Id;
            var invalidProductException = new InvalidProductException();

            invalidProductException.AddData(
                key: nameof(Product.UpdatedDate),
                values: $"Date is the same as {nameof(Product.UpdatedDate)}");

            var expectedProductValidationException =
                new ProductValidationException(invalidProductException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectProductByIdAsync(invalidProduct.Id)).ReturnsAsync(storageProduct);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDateTime);

            // when
            ValueTask<Product> modifyProductTask =
                this.productService.ModifyProductAsync(invalidProduct);

            ProductValidationException actualProductValidationException =
                await Assert.ThrowsAsync<ProductValidationException>(modifyProductTask.AsTask);

            // then
            actualProductValidationException.Should()
                .BeEquivalentTo(expectedProductValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectProductByIdAsync(productId), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedProductValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
