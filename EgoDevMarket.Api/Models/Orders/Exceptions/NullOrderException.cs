// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using Xeptions;

namespace EgoDevMarket.Api.Models.Orders.Exceptions
{
    public class NullOrderException : Xeption
    {
        public NullOrderException()
            : base(message: "Order is null.")
        { }
    }
}

