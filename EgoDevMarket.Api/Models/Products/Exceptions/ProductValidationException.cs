// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using Xeptions;

namespace EgoDevMarket.Api.Models.Products.Exceptions
{
    public class ProductValidationException : Xeption
    {
        public ProductValidationException(Xeption innerException)
            : base(message: "Product validation error occured, fix the errors and try again.", innerException)
        { }
    }
}
