// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using Xeptions;

namespace EgoDevMarket.Api.Models.Orders.Exceptions
{
    public class OrderDependencyValidationException : Xeption
    {
        public OrderDependencyValidationException(Xeption innerException)
            : base(message: "Order dependency validation error occurred, fix the errors and try again.",
                  innerException)
        { }
    }
}
