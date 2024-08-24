// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace EgoDevMarket.Api.Models.Searchs.Exceptions
{
    public class AlreadyExistsSearchException : Xeption
    {
        public AlreadyExistsSearchException(Exception innerException)
            : base(message: "Search already exists.", innerException)
        { }
    }
}
