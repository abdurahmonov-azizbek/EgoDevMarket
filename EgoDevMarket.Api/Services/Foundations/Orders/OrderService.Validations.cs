// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using EgoDevMarket.Api.Models.Orders;
using EgoDevMarket.Api.Models.Orders.Exceptions;

namespace EgoDevMarket.Api.Services.Foundations.Orders
{
    public partial class OrderService
    {
        private void ValidateOrderOnAdd(Order order)
        {
            ValidateOrderNotNull(order);

            Validate(
				(Rule: IsInvalid(order.Id), Parameter: nameof(Order.Id)),
				(Rule: IsInvalid(order.CreatedDate), Parameter: nameof(Order.CreatedDate)),
				(Rule: IsInvalid(order.UpdatedDate), Parameter: nameof(Order.UpdatedDate)),

                (Rule: IsNotRecent(order.CreatedDate), Parameter: nameof(Order.CreatedDate)),

                (Rule: IsNotSame(
                    firstDate: order.CreatedDate,
                    secondDate: order.UpdatedDate,
                    secondDateName: nameof(Order.UpdatedDate)),

                    Parameter: nameof(Order.CreatedDate)));
        }

        private void ValidateOrderOnModify(Order order)
        {
            ValidateOrderNotNull(order);

            Validate(
				(Rule: IsInvalid(order.Id), Parameter: nameof(Order.Id)),
				(Rule: IsInvalid(order.CreatedDate), Parameter: nameof(Order.CreatedDate)),
				(Rule: IsInvalid(order.UpdatedDate), Parameter: nameof(Order.UpdatedDate)),

                (Rule: IsNotRecent(order.UpdatedDate), Parameter: nameof(Order.UpdatedDate)),

                (Rule: IsSame(
                    firstDate: order.UpdatedDate,
                    secondDate: order.CreatedDate,
                    secondDateName: nameof(order.CreatedDate)),
                    Parameter: nameof(order.UpdatedDate)));
        }

        private static void ValidateAgainstStorageOrderOnModify(Order inputOrder, Order storageOrder)
        {
            ValidateStorageOrder(storageOrder, inputOrder.Id);
            Validate(
            (Rule: IsNotSame(
                    firstDate: inputOrder.CreatedDate,
                    secondDate: storageOrder.CreatedDate,
                    secondDateName: nameof(Order.CreatedDate)),
                    Parameter: nameof(Order.CreatedDate)),

                     (Rule: IsSame(
                        firstDate: inputOrder.UpdatedDate,
                        secondDate: storageOrder.UpdatedDate,
                        secondDateName: nameof(Order.UpdatedDate)),
                        Parameter: nameof(Order.UpdatedDate)));
        }

        private static void ValidateStorageOrder(Order maybeOrder, Guid orderId)
        {
            if (maybeOrder is null)
            {
                throw new NotFoundOrderException(orderId);
            }
        }

        private void ValidateOrderId(Guid orderId) =>
             Validate((Rule: IsInvalid(orderId), Parameter: nameof(Order.Id)));

        private void ValidateOrderNotNull(Order order)
        {
            if (order is null)
            {
                throw new NullOrderException();
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
            var invalidOrderException = new InvalidOrderException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidOrderException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }
            invalidOrderException.ThrowIfContainsErrors();
        }
    }
}
