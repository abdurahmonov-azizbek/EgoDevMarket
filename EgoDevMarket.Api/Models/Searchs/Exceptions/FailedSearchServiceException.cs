// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace EgoDevMarket.Api.Models.Searchs.Exceptions
{
    public class FailedSearchServiceException : Xeption
    {
        public FailedSearchServiceException(Exception innerException)
            : base(message: "Failed search service error occurred, please contact support.", innerException)
        { }
    }
}