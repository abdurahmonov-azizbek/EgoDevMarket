// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EgoDevMarket.Api.Models.Products;
using EgoDevMarket.Api.Models.Products.Exceptions;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace EgoDevMarket.Api.Services.Foundations.Products
{
    public partial class ProductService
    {
        private delegate ValueTask<Product> ReturningProductFunction();
        private delegate IQueryable<Product> ReturningProductsFunction();

        private async ValueTask<Product> TryCatch(ReturningProductFunction returningProductFunction)
        {
            try
            {
                return await returningProductFunction();
            }
            catch (NullProductException nullProductException)
            {
                throw CreateAndLogValidationException(nullProductException);
            }
            catch (InvalidProductException invalidProductException)
            {
                throw CreateAndLogValidationException(invalidProductException);
            }
            catch (NotFoundProductException notFoundProductException)
            {
                throw CreateAndLogValidationException(notFoundProductException);
            }
            catch (SqlException sqlException)
            {
                var failedProductStorageException = new FailedProductStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedProductStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsProductException = new AlreadyExistsProductException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsProductException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedProductException = new LockedProductException(dbUpdateConcurrencyException);

                throw CreateAndLogDependencyValidationException(lockedProductException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedProductStorageException = new FailedProductStorageException(databaseUpdateException);

                throw CreateAndLogDependencyException(failedProductStorageException);
            }
            catch (Exception exception)
            {
                var failedProductServiceException = new FailedProductServiceException(exception);

                throw CreateAndLogServiceException(failedProductServiceException);
            }
        }

        private IQueryable<Product> TryCatch(ReturningProductsFunction returningProductsFunction)
        {
            try
            {
                return returningProductsFunction();
            }
            catch (SqlException sqlException)
            {
                var failedProductStorageException = new FailedProductStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedProductStorageException);
            }
            catch (Exception serviceException)
            {
                var failedProductServiceException = new FailedProductServiceException(serviceException);

                throw CreateAndLogServiceException(failedProductServiceException);
            }
        }

        private ProductValidationException CreateAndLogValidationException(Xeption exception)
        {
            var productValidationException = new ProductValidationException(exception);
            this.loggingBroker.LogError(productValidationException);

            return productValidationException;
        }

        private ProductDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var ProductDependencyException = new ProductDependencyException(exception);
            this.loggingBroker.LogCritical(ProductDependencyException);

            return ProductDependencyException;
        }

        private ProductDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var productDependencyException = new ProductDependencyException(exception);
            this.loggingBroker.LogError(productDependencyException);

            return productDependencyException;
        }


        private ProductDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var productDependencyValidationException = new ProductDependencyValidationException(exception);
            this.loggingBroker.LogError(productDependencyValidationException);

            return productDependencyValidationException;
        }

        private ProductServiceException CreateAndLogServiceException(Xeption innerException)
        {
            var productServiceException = new ProductServiceException(innerException);
            this.loggingBroker.LogError(productServiceException);

            return productServiceException;
        }
    }
}