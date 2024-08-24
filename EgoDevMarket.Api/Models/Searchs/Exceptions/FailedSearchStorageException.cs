// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace EgoDevMarket.Api.Models.Searchs.Exceptions
{
    public class FailedSearchStorageException : Xeption
    {
        public FailedSearchStorageException(Exception innerException)
            : base(message: "Failed search storage error occurred, contact support.", innerException)
        { }
    }
}