// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EgoDevMarket.Api.Models.Products;

namespace EgoDevMarket.Api.Services.Foundations.Products
{
    public interface IProductService  
    {
        /// <exception cref="Models.Products.Exceptions.ProductValidationException"></exception>
        /// <exception cref="Models.Products.Exceptions.ProductDependencyValidationException"></exception>
        /// <exception cref="Models.Products.Exceptions.ProductDependencyException"></exception>
        /// <exception cref="Models.Products.Exceptions.ProductServiceException"></exception>
        ValueTask<Product> AddProductAsync(Product product);

        /// <exception cref="Models.Products.Exceptions.ProductDependencyException"></exception>
        /// <exception cref="Models.Products.Exceptions.ProductServiceException"></exception>
        IQueryable<Product> RetrieveAllProducts();

        /// <exception cref="Models.Products.Exceptions.ProductDependencyException"></exception>
        /// <exception cref="Models.Products.Exceptions.ProductServiceException"></exception>
        ValueTask<Product> RetrieveProductByIdAsync(Guid productId);

        /// <exception cref="Models.Products.Exceptions.ProductValidationException"></exception>
        /// <exception cref="Models.Products.Exceptions.ProductDependencyValidationException"></exception>
        /// <exception cref="Models.Products.Exceptions.ProductDependencyException"></exception>
        /// <exception cref="Models.Products.Exceptions.ProductServiceException"></exception>
        ValueTask<Product> ModifyProductAsync(Product product);

        /// <exception cref="Models.Products.Exceptions.ProductDependencyValidationException"></exception>
        /// <exception cref="Models.Products.Exceptions.ProductDependencyException"></exception>
        /// <exception cref="Models.Products.Exceptions.ProductServiceException"></exception>
        ValueTask<Product> RemoveProductByIdAsync(Guid productId);
    }
}