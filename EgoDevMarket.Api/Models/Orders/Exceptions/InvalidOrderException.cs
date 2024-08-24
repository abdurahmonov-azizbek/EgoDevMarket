// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using Xeptions;

namespace EgoDevMarket.Api.Models.Orders.Exceptions
{
    public class InvalidOrderException : Xeption
    {
        public InvalidOrderException()
            : base(message: "Order is invalid.")
        { }
    }
}
