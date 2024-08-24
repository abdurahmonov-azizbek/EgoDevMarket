// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EgoDevMarket.Api.Models.Searchs;
using EgoDevMarket.Api.Models.Searchs.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace EgoDevMarket.Api.Services.Foundations.Searchs
{
    public partial class SearchService
    {
        private delegate ValueTask<Search> ReturningSearchFunction();
        private delegate IQueryable<Search> ReturningSearchsFunction();

        private async ValueTask<Search> TryCatch(ReturningSearchFunction returningSearchFunction)
        {
            try
            {
                return await returningSearchFunction();
            }
            catch (NullSearchException nullSearchException)
            {
                throw CreateAndLogValidationException(nullSearchException);
            }
            catch (InvalidSearchException invalidSearchException)
            {
                throw CreateAndLogValidationException(invalidSearchException);
            }
            catch (NotFoundSearchException notFoundSearchException)
            {
                throw CreateAndLogValidationException(notFoundSearchException);
            }
            catch (SqlException sqlException)
            {
                var failedSearchStorageException = new FailedSearchStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedSearchStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsSearchException = new AlreadyExistsSearchException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsSearchException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedSearchException = new LockedSearchException(dbUpdateConcurrencyException);

                throw CreateAndLogDependencyValidationException(lockedSearchException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedSearchStorageException = new FailedSearchStorageException(databaseUpdateException);

                throw CreateAndLogDependencyException(failedSearchStorageException);
            }
            catch (Exception exception)
            {
                var failedSearchServiceException = new FailedSearchServiceException(exception);

                throw CreateAndLogServiceException(failedSearchServiceException);
            }
        }

        private IQueryable<Search> TryCatch(ReturningSearchsFunction returningSearchsFunction)
        {
            try
            {
                return returningSearchsFunction();
            }
            catch (SqlException sqlException)
            {
                var failedSearchStorageException = new FailedSearchStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedSearchStorageException);
            }
            catch (Exception serviceException)
            {
                var failedSearchServiceException = new FailedSearchServiceException(serviceException);

                throw CreateAndLogServiceException(failedSearchServiceException);
            }
        }

        private SearchValidationException CreateAndLogValidationException(Xeption exception)
        {
            var searchValidationException = new SearchValidationException(exception);
            this.loggingBroker.LogError(searchValidationException);

            return searchValidationException;
        }

        private SearchDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var SearchDependencyException = new SearchDependencyException(exception);
            this.loggingBroker.LogCritical(SearchDependencyException);

            return SearchDependencyException;
        }

        private SearchDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var searchDependencyException = new SearchDependencyException(exception);
            this.loggingBroker.LogError(searchDependencyException);

            return searchDependencyException;
        }


        private SearchDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var searchDependencyValidationException = new SearchDependencyValidationException(exception);
            this.loggingBroker.LogError(searchDependencyValidationException);

            return searchDependencyValidationException;
        }

        private SearchServiceException CreateAndLogServiceException(Xeption innerException)
        {
            var searchServiceException = new SearchServiceException(innerException);
            this.loggingBroker.LogError(searchServiceException);

            return searchServiceException;
        }
    }
}