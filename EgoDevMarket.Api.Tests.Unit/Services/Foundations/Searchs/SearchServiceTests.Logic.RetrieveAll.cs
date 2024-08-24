// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System.Linq;
using EgoDevMarket.Api.Models.Searchs;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Xunit;

namespace EgoDevMarket.Api.Tests.Unit.Services.Foundations.Searchs
{
    public partial class SearchServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllSearchs()
        {
            //given
            IQueryable<Search> randomSearchs = CreateRandomSearchs();
            IQueryable<Search> storageSearchs = randomSearchs;
            IQueryable<Search> expectedSearchs = storageSearchs.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllSearchs()).Returns(storageSearchs);

            //when
            IQueryable<Search> actualSearchs =
                this.searchService.RetrieveAllSearchs();

            //then
            actualSearchs.Should().BeEquivalentTo(expectedSearchs);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllSearchs(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
