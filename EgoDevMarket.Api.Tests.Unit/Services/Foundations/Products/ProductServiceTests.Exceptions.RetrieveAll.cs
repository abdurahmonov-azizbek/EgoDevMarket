// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using EgoDevMarket.Api.Models.Products.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

namespace EgoDevMarket.Api.Tests.Unit.Services.Foundations.Products
{
    public partial class ProductServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            //given
            SqlException sqlException = CreateSqlException();

            var failedStorageException =
                new FailedProductStorageException(sqlException);

            var expectedProductDependencyException =
                new ProductDependencyException(failedStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllProducts()).Throws(sqlException);

            //when
            Action retrieveAllProductsAction = () =>
                this.productService.RetrieveAllProducts();

            ProductDependencyException actualProductDependencyException =
                Assert.Throws<ProductDependencyException>(retrieveAllProductsAction);

            //then
            actualProductDependencyException.Should().BeEquivalentTo(
                expectedProductDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllProducts(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedProductDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomString();
            var serviceException = new Exception(exceptionMessage);

            var failedProductServiceException =
                new FailedProductServiceException(serviceException);

            var expectedProductServiceException =
                new ProductServiceException(failedProductServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllProducts()).Throws(serviceException);

            // when
            Action retrieveAllProductsAction = () =>
                this.productService.RetrieveAllProducts();

            ProductServiceException actualProductServiceException =
                Assert.Throws<ProductServiceException>(retrieveAllProductsAction);

            // then
            actualProductServiceException.Should().BeEquivalentTo(expectedProductServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllProducts(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedProductServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}