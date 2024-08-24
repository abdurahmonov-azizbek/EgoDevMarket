// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using Xeptions;

namespace EgoDevMarket.Api.Models.Searchs.Exceptions
{
    public class SearchDependencyValidationException : Xeption
    {
        public SearchDependencyValidationException(Xeption innerException)
            : base(message: "Search dependency validation error occurred, fix the errors and try again.",
                  innerException)
        { }
    }
}
