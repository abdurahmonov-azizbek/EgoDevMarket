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
        public async Task ShouldRetrieveSearchByIdAsync()
        {
            //given
            Guid randomSearchId = Guid.NewGuid();
            Guid inputSearchId = randomSearchId;
            Search randomSearch = CreateRandomSearch();
            Search storageSearch = randomSearch;
            Search excpectedSearch = randomSearch.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSearchByIdAsync(inputSearchId)).ReturnsAsync(storageSearch);

            //when
            Search actuallSearch = await this.searchService.RetrieveSearchByIdAsync(inputSearchId);

            //then
            actuallSearch.Should().BeEquivalentTo(excpectedSearch);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSearchByIdAsync(inputSearchId), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}