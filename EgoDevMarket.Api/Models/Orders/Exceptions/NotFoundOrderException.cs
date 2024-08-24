// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace EgoDevMarket.Api.Models.Orders.Exceptions
{
    public class NotFoundOrderException : Xeption
    {
        public NotFoundOrderException(Guid orderId)
            : base(message: $"Couldn't find order with id: {orderId}.")
        { }
    }
}
