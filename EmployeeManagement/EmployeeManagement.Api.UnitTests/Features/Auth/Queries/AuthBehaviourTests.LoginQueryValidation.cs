using AutoFixture;
using Internship.EmployeeManagement.Application.Exceptions;
using Internship.EmployeeManagement.Application.Features.Auth.Queries.Common;
using Internship.EmployeeManagement.Application.Features.Auth.Queries.Login;
using Internship.EmployeeManagement.Core.Entities;
using MediatR;
using Moq;
using Shouldly;

namespace Internship.EmployeeManagement.Api.UnitTests.Features.Auth
{
    public partial class AuthHandlersTests
    {
        [Fact]
        public async Task ValidateUserBehaviour_WithValidCommand_ShouldProvideSuccessValidation()
        {
            var loginQueryValidationBehaviour = _fixture.Build<LoginQueryValidationBehaviour>().Create();

            var loginQuery = _fixture.Build<LoginQuery>().Create();

            var nextMock = new Mock<RequestHandlerDelegate<AuthQueryResponse>>();

            var returnedUser = _fixture.Build<UserEntity>()
                .With(x => x.Email, loginQuery.Email)
                .Create();

            var passwordCheckResult = true;

            SetupValidateUserBehaviourTests(returnedUser, loginQuery, passwordCheckResult);


            var result = await loginQueryValidationBehaviour.Handle(loginQuery, nextMock.Object, default);

            result.ShouldBeNull();

            _userManagerMock.Verify(x => x.FindByEmailAsync(It.Is<string>(x => x == loginQuery.Email)), Times.Once);
            _userManagerMock.Verify(x => x.CheckPasswordAsync(loginQuery.SharedUser, loginQuery.Password), Times.Once);

        }

        [Fact]
        public async Task ValidateUserBehaviour_WithNonExistentEmail_ShouldThrowException()
        {
            var loginQueryValidationBehaviour = _fixture.Build<LoginQueryValidationBehaviour>().Create();

            var loginQuery = _fixture.Build<LoginQuery>().Create();

            var nextMock = new Mock<RequestHandlerDelegate<AuthQueryResponse>>();

            var returnedUser = _fixture.Build<UserEntity>()
                .With(x => x.Email)
                .Create();

            var passwordCheckResult = true;

            SetupValidateUserBehaviourTests(returnedUser, loginQuery, passwordCheckResult);

            var handlerCall = async () => await loginQueryValidationBehaviour.Handle(loginQuery, nextMock.Object, default);

            handlerCall.ShouldThrow<BadRequestException>();

            _userManagerMock.Verify(x => x.FindByEmailAsync(It.Is<string>(x => x == loginQuery.Email)), Times.Once);
            _userManagerMock.Verify(x => x.CheckPasswordAsync(loginQuery.SharedUser, loginQuery.Password), Times.Never);

        }


        [Fact]
        public async Task ValidateUserBehaviour_WithInvalidPassword_ShouldThrowException()
        {
            var loginQueryValidationBehaviour = _fixture.Build<LoginQueryValidationBehaviour>().Create();

            var loginQuery = _fixture.Build<LoginQuery>().Create();

            var nextMock = new Mock<RequestHandlerDelegate<AuthQueryResponse>>();

            var returnedUser = _fixture.Build<UserEntity>()
                .With(x => x.Email, loginQuery.Email)
                .Create();

            var passwordCheckResult = false;

            SetupValidateUserBehaviourTests(returnedUser, loginQuery, passwordCheckResult);


            var handlerCall = async () => await loginQueryValidationBehaviour.Handle(loginQuery, nextMock.Object, default);

            handlerCall.ShouldThrow<BadRequestException>();

            _userManagerMock.Verify(x => x.FindByEmailAsync(It.Is<string>(x => x == loginQuery.Email)), Times.Once);
            _userManagerMock.Verify(x => x.CheckPasswordAsync(loginQuery.SharedUser, loginQuery.Password), Times.Never);

        }

        private void SetupValidateUserBehaviourTests(UserEntity returnedUser, LoginQuery loginQuery, bool passwordCheckResult)
        {
            _userManagerMock.Setup(x => x.FindByEmailAsync(returnedUser.Email)).ReturnsAsync(returnedUser);

            _userManagerMock.Setup(x => x.CheckPasswordAsync(returnedUser, loginQuery.Password)).ReturnsAsync(passwordCheckResult);
        }
    }
}
