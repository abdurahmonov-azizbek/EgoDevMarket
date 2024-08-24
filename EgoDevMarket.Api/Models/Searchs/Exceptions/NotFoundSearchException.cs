// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace EgoDevMarket.Api.Models.Searchs.Exceptions
{
    public class NotFoundSearchException : Xeption
    {
        public NotFoundSearchException(Guid searchId)
            : base(message: $"Couldn't find search with id: {searchId}.")
        { }
    }
}
