// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace EgoDevMarket.Api.Models.Products.Exceptions
{
    public class NotFoundProductException : Xeption
    {
        public NotFoundProductException(Guid productId)
            : base(message: $"Couldn't find product with id: {productId}.")
        { }
    }
}
