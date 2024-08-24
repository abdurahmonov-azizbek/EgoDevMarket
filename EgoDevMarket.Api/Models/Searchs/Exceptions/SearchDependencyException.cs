// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace EgoDevMarket.Api.Models.Searchs.Exceptions
{
    public class SearchDependencyException : Xeption
    {
        public SearchDependencyException(Exception innerException)
            : base(message: "Search dependency error occured, contact support.", innerException)
        { }
    }
}