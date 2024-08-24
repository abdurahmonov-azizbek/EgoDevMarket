// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using EgoDevMarket.Api.Models.Searchs;
using EgoDevMarket.Api.Models.Searchs.Exceptions;

namespace EgoDevMarket.Api.Services.Foundations.Searchs
{
    public partial class SearchService
    {
        private void ValidateSearchOnAdd(Search search)
        {
            ValidateSearchNotNull(search);

            Validate(
				(Rule: IsInvalid(search.Id{get;set;}), Parameter: nameof(Search.Id{get;set;})),
				(Rule: IsInvalid(search.Keyword{get;set;}), Parameter: nameof(Search.Keyword{get;set;})),
				(Rule: IsInvalid(search.CreatedDate), Parameter: nameof(Search.CreatedDate)),
				(Rule: IsInvalid(search.UpdatedDate), Parameter: nameof(Search.UpdatedDate)),

                (Rule: IsNotRecent(search.CreatedDate), Parameter: nameof(Search.CreatedDate)),

                (Rule: IsNotSame(
                    firstDate: search.CreatedDate,
                    secondDate: search.UpdatedDate,
                    secondDateName: nameof(Search.UpdatedDate)),

                    Parameter: nameof(Search.CreatedDate)));
        }

        private void ValidateSearchOnModify(Search search)
        {
            ValidateSearchNotNull(search);

            Validate(
				(Rule: IsInvalid(search.Id{get;set;}), Parameter: nameof(Search.Id{get;set;})),
				(Rule: IsInvalid(search.Keyword{get;set;}), Parameter: nameof(Search.Keyword{get;set;})),
				(Rule: IsInvalid(search.CreatedDate), Parameter: nameof(Search.CreatedDate)),
				(Rule: IsInvalid(search.UpdatedDate), Parameter: nameof(Search.UpdatedDate)),

                (Rule: IsNotRecent(search.UpdatedDate), Parameter: nameof(Search.UpdatedDate)),

                (Rule: IsSame(
                    firstDate: search.UpdatedDate,
                    secondDate: search.CreatedDate,
                    secondDateName: nameof(search.CreatedDate)),
                    Parameter: nameof(search.UpdatedDate)));
        }

        private static void ValidateAgainstStorageSearchOnModify(Search inputSearch, Search storageSearch)
        {
            ValidateStorageSearch(storageSearch, inputSearch.Id);
            Validate(
            (Rule: IsNotSame(
                    firstDate: inputSearch.CreatedDate,
                    secondDate: storageSearch.CreatedDate,
                    secondDateName: nameof(Search.CreatedDate)),
                    Parameter: nameof(Search.CreatedDate)),

                     (Rule: IsSame(
                        firstDate: inputSearch.UpdatedDate,
                        secondDate: storageSearch.UpdatedDate,
                        secondDateName: nameof(Search.UpdatedDate)),
                        Parameter: nameof(Search.UpdatedDate)));
        }

        private static void ValidateStorageSearch(Search maybeSearch, Guid searchId)
        {
            if (maybeSearch is null)
            {
                throw new NotFoundSearchException(searchId);
            }
        }

        private void ValidateSearchId(Guid searchId) =>
             Validate((Rule: IsInvalid(searchId), Parameter: nameof(Search.Id)));

        private void ValidateSearchNotNull(Search search)
        {
            if (search is null)
            {
                throw new NullSearchException();
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
            var invalidSearchException = new InvalidSearchException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidSearchException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }
            invalidSearchException.ThrowIfContainsErrors();
        }
    }
}
