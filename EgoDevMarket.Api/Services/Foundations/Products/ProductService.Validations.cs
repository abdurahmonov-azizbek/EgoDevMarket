// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using EgoDevMarket.Api.Models.Products;
using EgoDevMarket.Api.Models.Products.Exceptions;

namespace EgoDevMarket.Api.Services.Foundations.Products
{
    public partial class ProductService
    {
        private void ValidateProductOnAdd(Product product)
        {
            ValidateProductNotNull(product);

            Validate(
				(Rule: IsInvalid(product.Id), Parameter: nameof(Product.Id)),
				(Rule: IsInvalid(product.Name), Parameter: nameof(Product.Name)),
				(Rule: IsInvalid(product.Description{get;set;}), Parameter: nameof(Product.Description{get;set;})),
				(Rule: IsInvalid(product.CreatedDate), Parameter: nameof(Product.CreatedDate)),
				(Rule: IsInvalid(product.UpdatedDate), Parameter: nameof(Product.UpdatedDate)),

                (Rule: IsNotRecent(product.CreatedDate), Parameter: nameof(Product.CreatedDate)),

                (Rule: IsNotSame(
                    firstDate: product.CreatedDate,
                    secondDate: product.UpdatedDate,
                    secondDateName: nameof(Product.UpdatedDate)),

                    Parameter: nameof(Product.CreatedDate)));
        }

        private void ValidateProductOnModify(Product product)
        {
            ValidateProductNotNull(product);

            Validate(
				(Rule: IsInvalid(product.Id), Parameter: nameof(Product.Id)),
				(Rule: IsInvalid(product.Name), Parameter: nameof(Product.Name)),
				(Rule: IsInvalid(product.Description{get;set;}), Parameter: nameof(Product.Description{get;set;})),
				(Rule: IsInvalid(product.CreatedDate), Parameter: nameof(Product.CreatedDate)),
				(Rule: IsInvalid(product.UpdatedDate), Parameter: nameof(Product.UpdatedDate)),

                (Rule: IsNotRecent(product.UpdatedDate), Parameter: nameof(Product.UpdatedDate)),

                (Rule: IsSame(
                    firstDate: product.UpdatedDate,
                    secondDate: product.CreatedDate,
                    secondDateName: nameof(product.CreatedDate)),
                    Parameter: nameof(product.UpdatedDate)));
        }

        private static void ValidateAgainstStorageProductOnModify(Product inputProduct, Product storageProduct)
        {
            ValidateStorageProduct(storageProduct, inputProduct.Id);
            Validate(
            (Rule: IsNotSame(
                    firstDate: inputProduct.CreatedDate,
                    secondDate: storageProduct.CreatedDate,
                    secondDateName: nameof(Product.CreatedDate)),
                    Parameter: nameof(Product.CreatedDate)),

                     (Rule: IsSame(
                        firstDate: inputProduct.UpdatedDate,
                        secondDate: storageProduct.UpdatedDate,
                        secondDateName: nameof(Product.UpdatedDate)),
                        Parameter: nameof(Product.UpdatedDate)));
        }

        private static void ValidateStorageProduct(Product maybeProduct, Guid productId)
        {
            if (maybeProduct is null)
            {
                throw new NotFoundProductException(productId);
            }
        }

        private void ValidateProductId(Guid productId) =>
             Validate((Rule: IsInvalid(productId), Parameter: nameof(Product.Id)));

        private void ValidateProductNotNull(Product product)
        {
            if (product is null)
            {
                throw new NullProductException();
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
            var invalidProductException = new InvalidProductException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidProductException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }
            invalidProductException.ThrowIfContainsErrors();
        }
    }
}
