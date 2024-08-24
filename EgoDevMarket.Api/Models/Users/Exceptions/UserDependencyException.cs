// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using Xeptions;

namespace EgoDevMarket.Api.Models.Users.Exceptions
{
    public class UserDependencyException : Xeption
    {
        public UserDependencyException(Exception innerException)
            : base(message: "User dependency error occured, contact support.", innerException)
        { }
    }
}