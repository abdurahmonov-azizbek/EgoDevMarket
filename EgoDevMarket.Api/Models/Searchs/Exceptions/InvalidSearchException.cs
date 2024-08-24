// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using Xeptions;

namespace EgoDevMarket.Api.Models.Searchs.Exceptions
{
    public class InvalidSearchException : Xeption
    {
        public InvalidSearchException()
            : base(message: "Search is invalid.")
        { }
    }
}
