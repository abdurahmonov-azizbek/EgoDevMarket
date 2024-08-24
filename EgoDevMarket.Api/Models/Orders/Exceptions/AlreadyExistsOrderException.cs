// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace EgoDevMarket.Api.Models.Orders.Exceptions
{
    public class AlreadyExistsOrderException : Xeption
    {
        public AlreadyExistsOrderException(Exception innerException)
            : base(message: "Order already exists.", innerException)
        { }
    }
}
