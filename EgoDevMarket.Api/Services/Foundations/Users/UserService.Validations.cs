// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using EgoDevMarket.Api.Models.Users;
using EgoDevMarket.Api.Models.Users.Exceptions;

namespace EgoDevMarket.Api.Services.Foundations.Users
{
    public partial class UserService
    {
        private void ValidateUserOnAdd(User user)
        {
            ValidateUserNotNull(user);

            Validate(
				(Rule: IsInvalid(user.Id), Parameter: nameof(User.Id)),
				(Rule: IsInvalid(user.FirstName), Parameter: nameof(User.FirstName)),
				(Rule: IsInvalid(user.LastName), Parameter: nameof(User.LastName)),
				(Rule: IsInvalid(user.PhoneNumber{get;set;}), Parameter: nameof(User.PhoneNumber{get;set;})),
				(Rule: IsInvalid(user.Role{get;set;}), Parameter: nameof(User.Role{get;set;})),
				(Rule: IsInvalid(user.Password{get;set;}), Parameter: nameof(User.Password{get;set;})),
				(Rule: IsInvalid(user.CreatedDate), Parameter: nameof(User.CreatedDate)),
				(Rule: IsInvalid(user.UpdatedDate), Parameter: nameof(User.UpdatedDate)),

                (Rule: IsNotRecent(user.CreatedDate), Parameter: nameof(User.CreatedDate)),

                (Rule: IsNotSame(
                    firstDate: user.CreatedDate,
                    secondDate: user.UpdatedDate,
                    secondDateName: nameof(User.UpdatedDate)),

                    Parameter: nameof(User.CreatedDate)));
        }

        private void ValidateUserOnModify(User user)
        {
            ValidateUserNotNull(user);

            Validate(
				(Rule: IsInvalid(user.Id), Parameter: nameof(User.Id)),
				(Rule: IsInvalid(user.FirstName), Parameter: nameof(User.FirstName)),
				(Rule: IsInvalid(user.LastName), Parameter: nameof(User.LastName)),
				(Rule: IsInvalid(user.PhoneNumber{get;set;}), Parameter: nameof(User.PhoneNumber{get;set;})),
				(Rule: IsInvalid(user.Role{get;set;}), Parameter: nameof(User.Role{get;set;})),
				(Rule: IsInvalid(user.Password{get;set;}), Parameter: nameof(User.Password{get;set;})),
				(Rule: IsInvalid(user.CreatedDate), Parameter: nameof(User.CreatedDate)),
				(Rule: IsInvalid(user.UpdatedDate), Parameter: nameof(User.UpdatedDate)),

                (Rule: IsNotRecent(user.UpdatedDate), Parameter: nameof(User.UpdatedDate)),

                (Rule: IsSame(
                    firstDate: user.UpdatedDate,
                    secondDate: user.CreatedDate,
                    secondDateName: nameof(user.CreatedDate)),
                    Parameter: nameof(user.UpdatedDate)));
        }

        private static void ValidateAgainstStorageUserOnModify(User inputUser, User storageUser)
        {
            ValidateStorageUser(storageUser, inputUser.Id);
            Validate(
            (Rule: IsNotSame(
                    firstDate: inputUser.CreatedDate,
                    secondDate: storageUser.CreatedDate,
                    secondDateName: nameof(User.CreatedDate)),
                    Parameter: nameof(User.CreatedDate)),

                     (Rule: IsSame(
                        firstDate: inputUser.UpdatedDate,
                        secondDate: storageUser.UpdatedDate,
                        secondDateName: nameof(User.UpdatedDate)),
                        Parameter: nameof(User.UpdatedDate)));
        }

        private static void ValidateStorageUser(User maybeUser, Guid userId)
        {
            if (maybeUser is null)
            {
                throw new NotFoundUserException(userId);
            }
        }

        private void ValidateUserId(Guid userId) =>
             Validate((Rule: IsInvalid(userId), Parameter: nameof(User.Id)));

        private void ValidateUserNotNull(User user)
        {
            if (user is null)
            {
                throw new NullUserException();
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is required"
        };

        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not same as {secondDateName}"
            };

        private dynamic IsInvalid(string text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private static dynamic IsInvalid<T>(T value) => new
        {
            Condition = IsEnumInvalid(value),
            Message = "Value is not recognized"
        };

        private dynamic IsNotRecent(DateTimeOffset date) => new
        {
            Condition = IsDateNotRecent(date),
            Message = "Date is not recent"
        };

        private static bool IsEnumInvalid<T>(T value)
        {
            bool isDefined = Enum.IsDefined(typeof(T), value);

            return isDefined is false;
        }

        private static dynamic IsSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is the same as {secondDateName}"
            };

        private bool IsDateNotRecent(DateTimeOffset date)
        {
            DateTimeOffset currentDate = this.dateTimeBroker.GetCurrentDateTimeOffset();
            TimeSpan timeDifference = currentDate.Subtract(date);

            return timeDifference.TotalSeconds is > 60 or < 0;
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidUserException = new InvalidUserException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidUserException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }
            invalidUserException.ThrowIfContainsErrors();
        }
    }
}
