using AutoFixture;
using Internship.EmployeeManagement.Application.Exceptions;
using Internship.EmployeeManagement.Application.Features.Users.Commands.CreateUser;
using Internship.EmployeeManagement.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using Shouldly;

namespace Internship.UserManagement.Api.UnitTests.Features.Users
{
    public partial class UserHandlersTests
    {
        [Fact]
        public async Task CreateUserHandler_WithValidCommand_ShouldReturnTheCreatedUserId()
        {
            var createUserHandler = _fixture.Build<CreateUserCommandHandler>().Create();

            var userToBeCreated = _fixture.Build<CreateUserCommand>().Create();
            UserEntity userEntity = _fixture.Build<UserEntity>().With(x => x.Email, userToBeCreated.Email).Create();

            var successResult = IdentityResult.Success;

            _mapperMock.Setup(x => x.Map<UserEntity>(userToBeCreated)).Returns(userEntity);

            _userManagerMock.Setup(x => x.CreateAsync(userEntity, userToBeCreated.Password)).ReturnsAsync(successResult);

            _userManagerMock.Setup(x => x.AddToRoleAsync(userEntity, "User")).ReturnsAsync(successResult);

            var result = await createUserHandler.Handle(userToBeCreated, default);

            result.Id.ShouldBe(userEntity.Id);

            _mapperMock.Verify(x => x.Map<UserEntity>(userToBeCreated), Times.Once);

            _userManagerMock.Verify(x => x.CreateAsync(userEntity, userToBeCreated.Password), Times.Once);

            _userManagerMock.Verify(x => x.AddToRoleAsync(It.Is<UserEntity>(x => x.Email == userToBeCreated.Email), "User"), Times.Once);
        }

        [Fact]
        public async Task CreateUserHandler_WhenCreationFails_ShouldThrowBadRequestException()
        {
            var createUserHandler = _fixture.Build<CreateUserCommandHandler>().Create();

            var userToBeCreated = _fixture.Build<CreateUserCommand>().Create();
            UserEntity userEntity = _fixture.Build<UserEntity>().With(x => x.Email, userToBeCreated.Email).Create();

            var failureResult = IdentityResult.Failed();

            var successResult = IdentityResult.Success;

            _mapperMock.Setup(x => x.Map<UserEntity>(userToBeCreated)).Returns(userEntity);

            _userManagerMock.Setup(x => x.CreateAsync(userEntity, userToBeCreated.Password)).ReturnsAsync(failureResult);

            _userManagerMock.Setup(x => x.AddToRoleAsync(userEntity, "User")).ReturnsAsync(successResult);

            var apiCall = async () => await createUserHandler.Handle(userToBeCreated, default);

            var exception = apiCall.ShouldThrow<BadRequestException>();

            exception.Message.ShouldBeNullOrEmpty();

            _mapperMock.Verify(x => x.Map<UserEntity>(userToBeCreated), Times.Once);

            _userManagerMock.Verify(x => x.CreateAsync(userEntity, userToBeCreated.Password), Times.Once);

            _userManagerMock.Verify(x => x.AddToRoleAsync(It.Is<UserEntity>(x => x.Email == userToBeCreated.Email), "User"), Times.Never);
        }

        [Fact]
        public async Task CreateUserHandler_WhenRoleAssignationFails_ShouldThrowBadRequestException()
        {
            var createUserHandler = _fixture.Build<CreateUserCommandHandler>().Create();

            var userToBeCreated = _fixture.Build<CreateUserCommand>().Create();
            UserEntity userEntity = _fixture.Build<UserEntity>().With(x => x.Email, userToBeCreated.Email).Create();

            var failureResult = IdentityResult.Failed();

            var successResult = IdentityResult.Success;

            _mapperMock.Setup(x => x.Map<UserEntity>(userToBeCreated)).Returns(userEntity);

            _userManagerMock.Setup(x => x.CreateAsync(userEntity, userToBeCreated.Password)).ReturnsAsync(successResult);

            _userManagerMock.Setup(x => x.AddToRoleAsync(userEntity, "User")).ReturnsAsync(failureResult);

            var apiCall = async () => await createUserHandler.Handle(userToBeCreated, default);

            var exception = apiCall.ShouldThrow<BadRequestException>();

            exception.Message.ShouldBeNullOrEmpty();

            _mapperMock.Verify(x => x.Map<UserEntity>(userToBeCreated), Times.Once);

            _userManagerMock.Verify(x => x.CreateAsync(userEntity, userToBeCreated.Password), Times.Once);

            _userManagerMock.Verify(x => x.AddToRoleAsync(It.Is<UserEntity>(x => x.Email == userToBeCreated.Email), "User"), Times.Once);
        }
    }
}
