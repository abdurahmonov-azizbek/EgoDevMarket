// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace EgoDevMarket.Api.Models.Orders.Exceptions
{
    public class FailedOrderStorageException : Xeption
    {
        public FailedOrderStorageException(Exception innerException)
            : base(message: "Failed order storage error occurred, contact support.", innerException)
        { }
    }
}