// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace EgoDevMarket.Api.Models.Products.Exceptions
{
    public class AlreadyExistsProductException : Xeption
    {
        public AlreadyExistsProductException(Exception innerException)
            : base(message: "Product already exists.", innerException)
        { }
    }
}
