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
using Moq;
using Xunit;

namespace EgoDevMarket.Api.Tests.Unit.Services.Foundations.Searchs
{
    public partial class SearchServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedSearchStorageException =
                new FailedSearchStorageException(sqlException);

            var expectedSearchDependencyException =
                new SearchDependencyException(failedSearchStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSearchByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            //when
            ValueTask<Search> retrieveSearchByIdTask =
                this.searchService.RetrieveSearchByIdAsync(someId);

            SearchDependencyException actualSearchDependencyexception =
                await Assert.ThrowsAsync<SearchDependencyException>(
                    retrieveSearchByIdTask.AsTask);

            //then
            actualSearchDependencyexception.Should().BeEquivalentTo(
                expectedSearchDependencyException);

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
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdAsyncIfServiceErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedSearchServiceException =
                new FailedSearchServiceException(serviceException);

            var expectedSearchServiceException =
                new SearchServiceException(failedSearchServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSearchByIdAsync(It.IsAny<Guid>())).ThrowsAsync(serviceException);

            //when
            ValueTask<Search> retrieveSearchByIdTask =
                this.searchService.RetrieveSearchByIdAsync(someId);

            SearchServiceException actualSearchServiceException =
                await Assert.ThrowsAsync<SearchServiceException>(retrieveSearchByIdTask.AsTask);

            //then
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