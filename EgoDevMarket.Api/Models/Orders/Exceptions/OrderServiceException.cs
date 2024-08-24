// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace EgoDevMarket.Api.Models.Orders.Exceptions
{
    public class OrderServiceException : Xeption
    {
        public OrderServiceException(Exception innerException)
            : base(message: "Order service error occured, contact support.", innerException)
        { }
    }
}