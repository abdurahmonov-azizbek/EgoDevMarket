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
        public async Task ShouldModifySearchAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDateTime();
            Search randomSearch = CreateRandomModifySearch(randomDate);
            Search inputSearch = randomSearch;
            Search storageSearch = inputSearch.DeepClone();
            storageSearch.UpdatedDate = randomSearch.CreatedDate;
            Search updatedSearch = inputSearch;
            Search expectedSearch = updatedSearch.DeepClone();
            Guid searchId = inputSearch.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSearchByIdAsync(searchId))
                    .ReturnsAsync(storageSearch);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateSearchAsync(inputSearch))
                    .ReturnsAsync(updatedSearch);

            // when
            Search actualSearch =
               await this.searchService.ModifySearchAsync(inputSearch);

            // then
            actualSearch.Should().BeEquivalentTo(expectedSearch);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSearchByIdAsync(searchId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSearchAsync(inputSearch), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
