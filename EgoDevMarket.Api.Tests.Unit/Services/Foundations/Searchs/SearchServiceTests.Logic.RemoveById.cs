// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
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
        public async Task ShouldRemoveSearchByIdAsync()
        {
            // given
            Guid randomSearchId = Guid.NewGuid();
            Guid inputSearchId = randomSearchId;
            Search randomSearch = CreateRandomSearch();
            Search storageSearch = randomSearch;
            Search expectedInputSearch = storageSearch;
            Search deletedSearch = expectedInputSearch;
            Search expectedSearch = deletedSearch.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSearchByIdAsync(inputSearchId))
                    .ReturnsAsync(storageSearch);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteSearchAsync(expectedInputSearch))
                    .ReturnsAsync(deletedSearch);

            // when
            Search actualSearch = await this.searchService
                .RemoveSearchByIdAsync(inputSearchId);

            // then
            actualSearch.Should().BeEquivalentTo(expectedSearch);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSearchByIdAsync(inputSearchId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteSearchAsync(expectedInputSearch), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
