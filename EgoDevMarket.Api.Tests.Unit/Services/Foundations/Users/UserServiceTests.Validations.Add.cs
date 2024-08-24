// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Threading.Tasks;
using EgoDevMarket.Api.Models.Users;
using EgoDevMarket.Api.Models.Users.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace EgoDevMarket.Api.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            // given
            User nullUser = null;
            var nullUserException = new NullUserException();

            var expectedUserValidationException =
                new UserValidationException(nullUserException);

            // when
            ValueTask<User> addUserTask = this.userService.AddUserAsync(nullUser);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(addUserTask.AsTask);

            // then
            actualUserValidationException.Should()
                .BeEquivalentTo(expectedUserValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(
                    SameExceptionAs(expectedUserValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserAsync(It.IsAny<User>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfJobIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            User invalidUser = new User()
            {
                FirstName = invalidText
            };

            var invalidUserException = new InvalidUserException();

				invalidUserException.AddData(
					key: nameof(User.Id),
					values: "Id is required");

				invalidUserException.AddData(
					key: nameof(User.FirstName),
					values: "Text is required");

				invalidUserException.AddData(
					key: nameof(User.LastName),
					values: "Text is required");

				invalidUserException.AddData(
					key: nameof(User.PhoneNumber{get;set;}),
					values: "Text is required");

				invalidUserException.AddData(
					key: nameof(User.Role{get;set;}),
					values: "Text is required");

				invalidUserException.AddData(
					key: nameof(User.Password{get;set;}),
					values: "Text is required");

				invalidUserException.AddData(
					key: nameof(User.CreatedDate),
					values: "Date is required");

				invalidUserException.AddData(
					key: nameof(User.UpdatedDate),
					values: "Date is required");



            var expectedUserValidationException =
                new UserValidationException(invalidUserException);

            // when
            ValueTask<User> addUserTask = this.userService.AddUserAsync(invalidUser);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(addUserTask.AsTask);

            // then
            actualUserValidationException.Should()
                .BeEquivalentTo(expectedUserValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserAsync(It.IsAny<User>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShoudlThrowValidationExceptionOnAddIfCreatedDateIsNotSameAsUpdatedDateAndLogItAsync()
        {
            // given
            int randomMinutes = GetRandomNumber();
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            User randomUser = CreateRandomUser(randomDate);
            User invalidUser = randomUser;
            invalidUser.UpdatedDate = randomDate.AddMinutes(randomMinutes);
            var invalidUserException = new InvalidUserException();

            invalidUserException.AddData(
                key: nameof(User.CreatedDate),
                values: $"Date is not same as {nameof(User.UpdatedDate)}");

            var expectedUserValidationException = new UserValidationException(invalidUserException);

            this.dateTimeBrokerMock.Setup(broker => broker.GetCurrentDateTimeOffset())
                .Returns(randomDate);

            // when
            ValueTask<User> addUserTask = this.userService.AddUserAsync(invalidUser);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(addUserTask.AsTask);

            // then
            actualUserValidationException.Should().BeEquivalentTo(expectedUserValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedUserValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserAsync(It.IsAny<User>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidMinutes))]
        public async Task ShouldThrowValidationExceptionIfCreatedDateIsNotRecentAndLogItAsync(
            int invalidMinutes)
        {
            // given
            DateTimeOffset randomDate = GetRandomDatetimeOffset();
            DateTimeOffset invalidDateTime = randomDate.AddMinutes(invalidMinutes);
            User randomUser = CreateRandomUser(invalidDateTime);
            User invalidUser = randomUser;
            var invalidUserException = new InvalidUserException();

            invalidUserException.AddData(
                key: nameof(User.CreatedDate),
                values: "Date is not recent");

            var expectedUserValidationException =
                new UserValidationException(invalidUserException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset()).Returns(randomDate);

            // when
            ValueTask<User> addUserTask = this.userService.AddUserAsync(invalidUser);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(addUserTask.AsTask);

            // then
            actualUserValidationException.Should().
                BeEquivalentTo(expectedUserValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
            broker.GetCurrentDateTimeOffset(), Times.Once);

            this.loggingBrokerMock.Verify(broker => broker.LogError(It.Is(
                SameExceptionAs(expectedUserValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserAsync(It.IsAny<User>()), Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}