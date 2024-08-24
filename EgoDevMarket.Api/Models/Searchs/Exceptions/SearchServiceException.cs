// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace EgoDevMarket.Api.Models.Searchs.Exceptions
{
    public class SearchServiceException : Xeption
    {
        public SearchServiceException(Exception innerException)
            : base(message: "Search service error occured, contact support.", innerException)
        { }
    }
}