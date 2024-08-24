// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace EgoDevMarket.Api.Models.Orders.Exceptions
{
    public class LockedOrderException : Xeption
    {
        public LockedOrderException(Exception innerException)
            : base(message: "Order is locked, please try again.", innerException)
        { }
    }
}
