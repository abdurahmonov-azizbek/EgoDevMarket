// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using EgoDevMarket.Api.Models.Searchs;
using EgoDevMarket.Api.Models.Searchs.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EgoDevMarket.Api.Tests.Unit.Services.Foundations.Searchs
{
    public partial class SearchServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            DateTimeOffset someDateTime = GetRandomDateTime();
            Search randomSearch = CreateRandomSearch();
            Search someSearch = randomSearch;
            Guid searchId = someSearch.Id;
            SqlException sqlException = CreateSqlException();

            var failedSearchStorageException =
                new FailedSearchStorageException(sqlException);

            var expectedSearchDependencyException =
                new SearchDependencyException(failedSearchStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(sqlException);

            // when
            ValueTask<Search> modifySearchTask =
                this.searchService.ModifySearchAsync(someSearch);

            SearchDependencyException actualSearchDependencyException =
                await Assert.ThrowsAsync<SearchDependencyException>(
                    modifySearchTask.AsTask);

            // then
            actualSearchDependencyException.Should().BeEquivalentTo(
                expectedSearchDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSearchByIdAsync(searchId), Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSearchAsync(someSearch), Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedSearchDependencyException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
               broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            int minutesInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDatetimeOffset();
            Search randomSearch = CreateRandomSearch(randomDateTime);
            Search someSearch = randomSearch;
            someSearch.CreatedDate = someSearch.CreatedDate.AddMinutes(minutesInPast);
            var databaseUpdateException = new DbUpdateException();

            var failedStorageSearchException =
                new FailedSearchStorageException(databaseUpdateException);

            var expectedSearchDependencyException =
                new SearchDependencyException(failedStorageSearchException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateException);

            // when
            ValueTask<Search> modifySearchTask =
                this.searchService.ModifySearchAsync(someSearch);

            SearchDependencyException actualSearchDependencyException =
                await Assert.ThrowsAsync<SearchDependencyException>(
                    modifySearchTask.AsTask);

            // then
            actualSearchDependencyException.Should().BeEquivalentTo(expectedSearchDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSearchDependencyException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            int minutesInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDateTime();
            Search randomSearch = CreateRandomSearch(randomDateTime);
            Search someSearch = randomSearch;
            someSearch.CreatedDate = randomDateTime.AddMinutes(minutesInPast);
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedSearchException =
                new LockedSearchException(databaseUpdateConcurrencyException);

            var expectedSearchDependencyValidationException =
                new SearchDependencyValidationException(lockedSearchException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateConcurrencyException);

            // when
            ValueTask<Search> modifySearchTask =
                this.searchService.ModifySearchAsync(someSearch);

            SearchDependencyValidationException actualSearchDependencyValidationException =
                await Assert.ThrowsAsync<SearchDependencyValidationException>(modifySearchTask.AsTask);

            // then
            actualSearchDependencyValidationException.Should()
                .BeEquivalentTo(expectedSearchDependencyValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSearchDependencyValidationException))), Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            int minutesInPast = GetRandomNegativeNumber();
            var randomDateTime = GetRandomDateTime();
            Search randomSearch = CreateRandomSearch(randomDateTime);
            Search someSearch = randomSearch;
            someSearch.CreatedDate = someSearch.CreatedDate.AddMinutes(minutesInPast);
            var serviceException = new Exception();

            var failedSearchServiceException =
                new FailedSearchServiceException(serviceException);

            var expectedSearchServiceException =
                new SearchServiceException(failedSearchServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(serviceException);

            // when
            ValueTask<Search> modifySearchTask =
                this.searchService.ModifySearchAsync(someSearch);

            SearchServiceException actualSearchServiceException =
                await Assert.ThrowsAsync<SearchServiceException>(
                    modifySearchTask.AsTask);

            // then
            actualSearchServiceException.Should().BeEquivalentTo(expectedSearchServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSearchServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
