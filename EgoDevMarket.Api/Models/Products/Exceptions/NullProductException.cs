// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using Xeptions;

namespace EgoDevMarket.Api.Models.Products.Exceptions
{
    public class NullProductException : Xeption
    {
        public NullProductException()
            : base(message: "Product is null.")
        { }
    }
}

