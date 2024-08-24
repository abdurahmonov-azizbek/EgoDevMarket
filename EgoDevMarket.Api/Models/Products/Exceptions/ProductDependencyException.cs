// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace EgoDevMarket.Api.Models.Products.Exceptions
{
    public class ProductDependencyException : Xeption
    {
        public ProductDependencyException(Exception innerException)
            : base(message: "Product dependency error occured, contact support.", innerException)
        { }
    }
}