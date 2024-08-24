// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using Xeptions;

namespace EgoDevMarket.Api.Models.Searchs.Exceptions
{
    public class SearchValidationException : Xeption
    {
        public SearchValidationException(Xeption innerException)
            : base(message: "Search validation error occured, fix the errors and try again.", innerException)
        { }
    }
}
