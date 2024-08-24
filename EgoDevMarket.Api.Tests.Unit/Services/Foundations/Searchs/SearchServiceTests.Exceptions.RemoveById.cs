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
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someSearchId = Guid.NewGuid();

            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedSearchException =
                new LockedSearchException(databaseUpdateConcurrencyException);

            var expectedSearchDependencyValidationException =
                new SearchDependencyValidationException(lockedSearchException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSearchByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<Search> removeSearchByIdTask =
               this.searchService.RemoveSearchByIdAsync(someSearchId);

            SearchDependencyValidationException actualSearchDependencyValidationException =
                await Assert.ThrowsAsync<SearchDependencyValidationException>(
                    removeSearchByIdTask.AsTask);

            // then
            actualSearchDependencyValidationException.Should().BeEquivalentTo(
               expectedSearchDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSearchByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSearchDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteSearchAsync(It.IsAny<Search>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveWhenSqlExceptionOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedSearchStorageException =
                new FailedSearchStorageException(sqlException);

            var expectedSearchDependencyException =
                new SearchDependencyException(failedSearchStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSearchByIdAsync(someId))
                    .Throws(sqlException);
            // when
            ValueTask<Search> removeSearchTask =
                this.searchService.RemoveSearchByIdAsync(someId);

            SearchDependencyException actualSearchDependencyException =
                await Assert.ThrowsAsync<SearchDependencyException>(
                    removeSearchTask.AsTask);

            // then
            actualSearchDependencyException.Should().BeEquivalentTo(expectedSearchDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSearchByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedSearchDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someSearchId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedSearchServiceException =
                new FailedSearchServiceException(serviceException);

            var expectedSearchServiceException =
                new SearchServiceException(failedSearchServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSearchByIdAsync(someSearchId))
                    .Throws(serviceException);

            // when
            ValueTask<Search> removeSearchByIdTask =
                this.searchService.RemoveSearchByIdAsync(someSearchId);

            SearchServiceException actualSearchServiceException =
                await Assert.ThrowsAsync<SearchServiceException>(
                    removeSearchByIdTask.AsTask);

            // then
            actualSearchServiceException.Should().BeEquivalentTo(expectedSearchServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSearchByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSearchServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}