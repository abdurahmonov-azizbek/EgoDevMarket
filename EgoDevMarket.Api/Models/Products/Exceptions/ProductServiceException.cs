// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace EgoDevMarket.Api.Models.Products.Exceptions
{
    public class ProductServiceException : Xeption
    {
        public ProductServiceException(Exception innerException)
            : base(message: "Product service error occured, contact support.", innerException)
        { }
    }
}