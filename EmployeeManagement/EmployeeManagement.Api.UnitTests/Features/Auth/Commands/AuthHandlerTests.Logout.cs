using AutoFixture;
using Internship.EmployeeManagement.Application.Features.Auth.Commands.Logout;
using MediatR;
using Moq;
using Shouldly;
using System.Security.Claims;

namespace Internship.EmployeeManagement.Api.UnitTests.Features.Auth
{
    public partial class AuthHandlersTests
    {
        [Theory]
        [InlineData(1)]
        public async Task LogoutHandler_WithAccessTokenSentInHeader_ShouldReturnUnit(int userId)
        {

            List<Claim> claims = [new Claim(ClaimTypes.NameIdentifier, userId.ToString())];

            SetHttpContextAccessorUserForMock(claims);

            var logoutHandler = _fixture.Build<LogoutCommandHandler>().Create();

            var logoutCommand = _fixture.Build<LogoutCommand>().Create();

            _refreshTokenRepositoryMock.Setup(x => x.RemoveRefreshTokensByUserIdAsync(userId));

            var result = await logoutHandler.Handle(logoutCommand, default);

            result.ShouldBe(Unit.Value);

            _refreshTokenRepositoryMock.Verify(x => x.RemoveRefreshTokensByUserIdAsync(It.Is<int>(x => x == userId)), Times.Once);

        }



        [Theory]
        [InlineData(1)]
        public async Task LogoutHandler_WithNoAccessToken_ShouldReturnWithNoActionDone(int userId)
        {
            List<Claim> claims = new();

            SetHttpContextAccessorUserForMock(claims);

            var logoutHandler = _fixture.Build<LogoutCommandHandler>().Create();

            var logoutCommand = _fixture.Build<LogoutCommand>().Create();

            _refreshTokenRepositoryMock.Setup(x => x.RemoveRefreshTokensByUserIdAsync(userId));

            var result = await logoutHandler.Handle(logoutCommand, default);

            result.ShouldBe(Unit.Value);

            _refreshTokenRepositoryMock.Verify(x => x.RemoveRefreshTokensByUserIdAsync(userId), Times.Never);

        }


    }
}
