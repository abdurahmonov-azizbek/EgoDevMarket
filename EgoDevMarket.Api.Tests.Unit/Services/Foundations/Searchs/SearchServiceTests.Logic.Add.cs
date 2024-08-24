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
        public async Task ShouldAddSearchAsync()
        {
            // given
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            Search randomSearch = CreateRandomSearch(randomDate);
            Search inputSearch = randomSearch;
            Search persistedSearch = inputSearch;
            Search expectedSearch = persistedSearch.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertSearchAsync(inputSearch)).ReturnsAsync(persistedSearch);

            // when
            Search actualSearch = await this.searchService
                .AddSearchAsync(inputSearch);

            // then
            actualSearch.Should().BeEquivalentTo(expectedSearch);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSearchAsync(inputSearch), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}