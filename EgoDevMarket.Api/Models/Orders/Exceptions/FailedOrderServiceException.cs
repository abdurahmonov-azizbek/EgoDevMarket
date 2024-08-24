// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace EgoDevMarket.Api.Models.Orders.Exceptions
{
    public class FailedOrderServiceException : Xeption
    {
        public FailedOrderServiceException(Exception innerException)
            : base(message: "Failed order service error occurred, please contact support.", innerException)
        { }
    }
}