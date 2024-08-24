// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using Xeptions;

namespace EgoDevMarket.Api.Models.Products.Exceptions
{
    public class InvalidProductException : Xeption
    {
        public InvalidProductException()
            : base(message: "Product is invalid.")
        { }
    }
}
