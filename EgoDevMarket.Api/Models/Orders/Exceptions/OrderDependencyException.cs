// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace EgoDevMarket.Api.Models.Orders.Exceptions
{
    public class OrderDependencyException : Xeption
    {
        public OrderDependencyException(Exception innerException)
            : base(message: "Order dependency error occured, contact support.", innerException)
        { }
    }
}