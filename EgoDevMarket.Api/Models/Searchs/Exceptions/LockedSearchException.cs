// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace EgoDevMarket.Api.Models.Searchs.Exceptions
{
    public class LockedSearchException : Xeption
    {
        public LockedSearchException(Exception innerException)
            : base(message: "Search is locked, please try again.", innerException)
        { }
    }
}
