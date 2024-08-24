// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EgoDevMarket.Api.Models.Users;

namespace EgoDevMarket.Api.Services.Foundations.Users
{
    public interface IUserService  
    {
        /// <exception cref="Models.Users.Exceptions.UserValidationException"></exception>
        /// <exception cref="Models.Users.Exceptions.UserDependencyValidationException"></exception>
        /// <exception cref="Models.Users.Exceptions.UserDependencyException"></exception>
        /// <exception cref="Models.Users.Exceptions.UserServiceException"></exception>
        ValueTask<User> AddUserAsync(User user);

        /// <exception cref="Models.Users.Exceptions.UserDependencyException"></exception>
        /// <exception cref="Models.Users.Exceptions.UserServiceException"></exception>
        IQueryable<User> RetrieveAllUsers();

        /// <exception cref="Models.Users.Exceptions.UserDependencyException"></exception>
        /// <exception cref="Models.Users.Exceptions.UserServiceException"></exception>
        ValueTask<User> RetrieveUserByIdAsync(Guid userId);

        /// <exception cref="Models.Users.Exceptions.UserValidationException"></exception>
        /// <exception cref="Models.Users.Exceptions.UserDependencyValidationException"></exception>
        /// <exception cref="Models.Users.Exceptions.UserDependencyException"></exception>
        /// <exception cref="Models.Users.Exceptions.UserServiceException"></exception>
        ValueTask<User> ModifyUserAsync(User user);

        /// <exception cref="Models.Users.Exceptions.UserDependencyValidationException"></exception>
        /// <exception cref="Models.Users.Exceptions.UserDependencyException"></exception>
        /// <exception cref="Models.Users.Exceptions.UserServiceException"></exception>
        ValueTask<User> RemoveUserByIdAsync(Guid userId);
    }
}