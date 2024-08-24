// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using Xeptions;

namespace EgoDevMarket.Api.Models.Orders.Exceptions
{
    public class OrderValidationException : Xeption
    {
        public OrderValidationException(Xeption innerException)
            : base(message: "Order validation error occured, fix the errors and try again.", innerException)
        { }
    }
}
