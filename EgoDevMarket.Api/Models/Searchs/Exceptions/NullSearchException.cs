// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using Xeptions;

namespace EgoDevMarket.Api.Models.Searchs.Exceptions
{
    public class NullSearchException : Xeption
    {
        public NullSearchException()
            : base(message: "Search is null.")
        { }
    }
}

