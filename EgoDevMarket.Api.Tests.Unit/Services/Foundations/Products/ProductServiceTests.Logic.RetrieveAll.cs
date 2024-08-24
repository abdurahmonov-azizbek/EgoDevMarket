// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System.Linq;
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
        public void ShouldRetrieveAllProducts()
        {
            //given
            IQueryable<Product> randomProducts = CreateRandomProducts();
            IQueryable<Product> storageProducts = randomProducts;
            IQueryable<Product> expectedProducts = storageProducts.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllProducts()).Returns(storageProducts);

            //when
            IQueryable<Product> actualProducts =
                this.productService.RetrieveAllProducts();

            //then
            actualProducts.Should().BeEquivalentTo(expectedProducts);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllProducts(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
