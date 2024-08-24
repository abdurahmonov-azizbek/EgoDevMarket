// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using EgoDevMarket.Api.Models.Searchs;
using EgoDevMarket.Api.Models.Searchs.Exceptions;
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            Search someSearch = CreateRandomSearch();
            Guid searchId = someSearch.Id;
            SqlException sqlException = CreateSqlException();

            FailedSearchStorageException failedSearchStorageException =
                new FailedSearchStorageException(sqlException);

            SearchDependencyException expectedSearchDependencyException =
                new SearchDependencyException(failedSearchStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<Search> addSearchTask = this.searchService
                .AddSearchAsync(someSearch);

            SearchDependencyException actualSearchDependencyException =
                await Assert.ThrowsAsync<SearchDependencyException>(addSearchTask.AsTask);

            // then
            actualSearchDependencyException.Should().BeEquivalentTo(expectedSearchDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(expectedSearchDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorOccurredAndLogItAsync()
        {
            // given
            Search someSearch = CreateRandomSearch();
            string someMessage = GetRandomString();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsSearchException =
                new AlreadyExistsSearchException(duplicateKeyException);

            var expectedSearchDependencyValidationException =
                new SearchDependencyValidationException(alreadyExistsSearchException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(duplicateKeyException);

            // when
            ValueTask<Search> addSearchTask = this.searchService.AddSearchAsync(someSearch);

            SearchDependencyValidationException actualSearchDependencyValidationException =
                await Assert.ThrowsAsync<SearchDependencyValidationException>(
                    addSearchTask.AsTask);

            // then
            actualSearchDependencyValidationException.Should().BeEquivalentTo(
                expectedSearchDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSearchDependencyValidationException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateErrorOccursAndLogItAsync()
        {
            // given
            Search someSearch = CreateRandomSearch();
            var dbUpdateException = new DbUpdateException();

            var failedSearchStorageException = new FailedSearchStorageException(dbUpdateException);

            var expectedSearchDependencyException =
                new SearchDependencyException(failedSearchStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(dbUpdateException);

            // when
            ValueTask<Search> addSearchTask = this.searchService.AddSearchAsync(someSearch);

            SearchDependencyException actualSearchDependencyException =
                 await Assert.ThrowsAsync<SearchDependencyException>(addSearchTask.AsTask);

            // then
            actualSearchDependencyException.Should()
                .BeEquivalentTo(expectedSearchDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedSearchDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSearchAsync(It.IsAny<Search>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccuredAndLogItAsync()
        {
            // given
            Search someSearch = CreateRandomSearch();
            string someMessage = GetRandomString();

            var dbUpdateException =
                new DbUpdateException(someMessage);

            var failedSearchStorageException =
                new FailedSearchStorageException(dbUpdateException);

            var expectedSearchDependencyException =
                new SearchDependencyException(failedSearchStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                    broker.GetCurrentDateTimeOffset()).Throws(dbUpdateException);

            // when
            ValueTask<Search> addSearchTask =
                this.searchService.AddSearchAsync(someSearch);

            SearchDependencyException actualSearchDependencyException =
                await Assert.ThrowsAsync<SearchDependencyException>(addSearchTask.AsTask);

            // then
            actualSearchDependencyException.Should().BeEquivalentTo(expectedSearchDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSearchDependencyException))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccuredAndLogItAsync()
        {
            //given
            Search someSearch = CreateRandomSearch();
            var serviceException = new Exception();
            var failedSearchException = new FailedSearchServiceException(serviceException);

            var expectedSearchServiceExceptions =
                new SearchServiceException(failedSearchException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Throws(serviceException);

            //when
            ValueTask<Search> addSearchTask = this.searchService.AddSearchAsync(someSearch);

            SearchServiceException actualSearchServiceException =
                await Assert.ThrowsAsync<SearchServiceException>(addSearchTask.AsTask);

            //then
            actualSearchServiceException.Should().BeEquivalentTo(
                expectedSearchServiceExceptions);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSearchServiceExceptions))), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}