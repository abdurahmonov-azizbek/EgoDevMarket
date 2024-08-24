// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
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
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            //given
            SqlException sqlException = CreateSqlException();

            var failedStorageException =
                new FailedSearchStorageException(sqlException);

            var expectedSearchDependencyException =
                new SearchDependencyException(failedStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllSearchs()).Throws(sqlException);

            //when
            Action retrieveAllSearchsAction = () =>
                this.searchService.RetrieveAllSearchs();

            SearchDependencyException actualSearchDependencyException =
                Assert.Throws<SearchDependencyException>(retrieveAllSearchsAction);

            //then
            actualSearchDependencyException.Should().BeEquivalentTo(
                expectedSearchDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllSearchs(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedSearchDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomString();
            var serviceException = new Exception(exceptionMessage);

            var failedSearchServiceException =
                new FailedSearchServiceException(serviceException);

            var expectedSearchServiceException =
                new SearchServiceException(failedSearchServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllSearchs()).Throws(serviceException);

            // when
            Action retrieveAllSearchsAction = () =>
                this.searchService.RetrieveAllSearchs();

            SearchServiceException actualSearchServiceException =
                Assert.Throws<SearchServiceException>(retrieveAllSearchsAction);

            // then
            actualSearchServiceException.Should().BeEquivalentTo(expectedSearchServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllSearchs(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSearchServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}