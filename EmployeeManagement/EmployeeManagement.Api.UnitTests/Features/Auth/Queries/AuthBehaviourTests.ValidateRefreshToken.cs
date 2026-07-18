using AutoFixture;
using Internship.EmployeeManagement.Application.Exceptions;
using Internship.EmployeeManagement.Application.Features.Auth.Queries.Common;
using Internship.EmployeeManagement.Application.Features.Auth.Queries.RefreshToken;
using Internship.EmployeeManagement.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Shouldly;
using System.Security.Claims;

namespace Internship.EmployeeManagement.Api.UnitTests.Features.Auth
{
    public partial class AuthHandlersTests
    {
        [Fact]
        public async Task ValidateRefreshTokenBehaviour_WithValidAccessToken_ShouldReturnTheSameAccesToken()
        {
            List<Claim> claims = [ new Claim(ClaimTypes.NameIdentifier, "1") ];

            var accessToken = _fixture.Build<string>().Create();

            SetHttpContextAccessorRequestAndUserForMock(claims, accessToken);

            var validateRefreshTokenBehaviour = _fixture.Build<ValidateRefreshTokenBehaviour>().Create();

            var refreshQuery = _fixture.Build<RefreshTokenQuery>().Create();

            var nextMock = new Mock<RequestHandlerDelegate<AuthQueryResponse>>();


            var returnedRefreshToken = _fixture.Build<RefreshTokenEntity>()
                .With(x => x.Token, refreshQuery.RefreshToken)
                .Create();

            var returnedOldRefreshTokens = _fixture.Build<RefreshTokenEntity>()
                .With(x => x.UserId, returnedRefreshToken.UserId)
                .CreateMany(2).ToList();

            List<RefreshTokenEntity> oveallReturnedRefreshTokens = [..returnedOldRefreshTokens, returnedRefreshToken ];

            SetupRefreshTokenRepositoryMock(returnedRefreshToken, oveallReturnedRefreshTokens);


            var response = await validateRefreshTokenBehaviour.Handle(refreshQuery, nextMock.Object, default);

            response.AccessToken.ShouldBe(accessToken);

            _refreshTokenRepositoryMock.Verify(x => x.GetRefreshTokenByTokenAsync(returnedRefreshToken.Token), Times.Never);
        }



        [Fact]
        public async Task ValidateRefreshTokenBehaviour_WithNonExistingInvalidRefreshToken_ShouldThrowException()
        {
            SetHttpContextAccessorUserForMock(claims: new());

            var validateRefreshTokenBehaviour = _fixture.Build<ValidateRefreshTokenBehaviour>().Create();

            var refreshQuery = _fixture.Build<RefreshTokenQuery>().Create();

            var nextMock = new Mock<RequestHandlerDelegate<AuthQueryResponse>>();


            var returnedRefreshToken = _fixture.Build<RefreshTokenEntity>()
                .With(x => x.Token)
                .With(x => x.ExpiryDateUtc, DateTime.UtcNow.AddDays(3))
                .Create();

            var returnedOldRefreshTokens = _fixture.Build<RefreshTokenEntity>()
                .With(x => x.UserId, returnedRefreshToken.UserId)
                .CreateMany(2).ToList();

            List<RefreshTokenEntity> oveallReturnedRefreshTokens = [.. returnedOldRefreshTokens, returnedRefreshToken];

            SetupRefreshTokenRepositoryMock(returnedRefreshToken, oveallReturnedRefreshTokens);



            var handlerCall = async () => await validateRefreshTokenBehaviour.Handle(refreshQuery, nextMock.Object, default);



            var exception = handlerCall.ShouldThrow<BadRequestException>();

            exception.Message.ShouldBe("Invalid signature! Please, Login again");

            _refreshTokenRepositoryMock.Verify(x => x.GetRefreshTokensByUserIdAsync(returnedRefreshToken.UserId), Times.Never);

            _refreshTokenRepositoryMock.Verify(x => x.GetRefreshTokenByTokenAsync(refreshQuery.RefreshToken), Times.Once);

        }


        [Fact]
        public async Task ValidateRefreshTokenBehaviour_WithValidRefreshTokenButOldTokensStillExist_ShouldRemoveTheOldTokensAndBypassValidation()
        {
            SetHttpContextAccessorUserForMock(claims: new());

            var validateRefreshTokenBehaviour = _fixture.Build<ValidateRefreshTokenBehaviour>().Create();

            var refreshQuery = _fixture.Build<RefreshTokenQuery>().Create();

            var nextMock = new Mock<RequestHandlerDelegate<AuthQueryResponse>>();


            var returnedRefreshToken = _fixture.Build<RefreshTokenEntity>()
                .With(x => x.Token, refreshQuery.RefreshToken)
                .With(x => x.ExpiryDateUtc, DateTime.UtcNow.AddDays(3))
                .Create();

            var returnedOldRefreshTokens = _fixture.Build<RefreshTokenEntity>()
                .With(x => x.UserId, returnedRefreshToken.UserId)
                .With(x => x.ExpiryDateUtc, DateTime.UtcNow.AddDays(2))
                .CreateMany(2).ToList();

            returnedOldRefreshTokens[1].ExpiryDateUtc = DateTime.UtcNow.AddDays(1);

            List<RefreshTokenEntity> oveallReturnedRefreshTokens = [.. returnedOldRefreshTokens, returnedRefreshToken];

            SetupRefreshTokenRepositoryMock(returnedRefreshToken, oveallReturnedRefreshTokens);

            _refreshTokenRepositoryMock.Setup(x => x.RemoveRefreshToken(returnedRefreshToken));


            var response = await validateRefreshTokenBehaviour.Handle(refreshQuery, nextMock.Object, default);



            response.ShouldBeNull();


            _refreshTokenRepositoryMock.Verify(x => x.GetRefreshTokensByUserIdAsync(returnedRefreshToken.UserId), Times.Once);

            _refreshTokenRepositoryMock.Verify(x => x.RemoveVulnerableRefreshTokensAsync(It.Is<String>( x => x == returnedRefreshToken.Token), returnedRefreshToken.UserId), Times.Once);

            _refreshTokenRepositoryMock.Verify(x => x.RemoveRefreshToken(returnedRefreshToken), Times.Never);

        }


        [Fact]
        public async Task ValidateRefreshTokenBehaviour_WithValidOldRefreshToken_ShouldRemoveAllTokensAndThrowsException()
        {
            SetHttpContextAccessorUserForMock(claims: new());

            var validateRefreshTokenBehaviour = _fixture.Build<ValidateRefreshTokenBehaviour>().Create();

            var refreshQuery = _fixture.Build<RefreshTokenQuery>().Create();

            var nextMock = new Mock<RequestHandlerDelegate<AuthQueryResponse>>();


            var returnedRefreshToken = _fixture.Build<RefreshTokenEntity>()
                .With(x => x.Token, refreshQuery.RefreshToken)
                .With(x => x.ExpiryDateUtc, DateTime.UtcNow.AddDays(2))
                .Create();

            var returnedOldRefreshTokens = _fixture.Build<RefreshTokenEntity>()
                .With(x => x.UserId, returnedRefreshToken.UserId)
                .With(x => x.ExpiryDateUtc, DateTime.UtcNow.AddDays(3))
                .CreateMany(2).ToList();

            returnedOldRefreshTokens[1].ExpiryDateUtc = DateTime.UtcNow.AddDays(1);

            List<RefreshTokenEntity> oveallReturnedRefreshTokens = [.. returnedOldRefreshTokens, returnedRefreshToken];

            SetupRefreshTokenRepositoryMock(returnedRefreshToken, oveallReturnedRefreshTokens);


            var handlerCall = async () => await validateRefreshTokenBehaviour.Handle(refreshQuery, nextMock.Object, default);


            var exception = handlerCall.ShouldThrow<BadRequestException>();

            _refreshTokenRepositoryMock.Verify(x => x.GetRefreshTokensByUserIdAsync(returnedRefreshToken.UserId), Times.Once);

            _refreshTokenRepositoryMock.Verify(x => x.RemoveVulnerableRefreshTokensAsync(It.Is<String>(x => x == returnedRefreshToken.Token), returnedRefreshToken.UserId), Times.Never);

            _refreshTokenRepositoryMock.Verify(x => x.GetRefreshTokensByUserIdAsync(returnedRefreshToken.UserId), Times.Once);

        }



        [Fact]
        public async Task ValidateRefreshTokenBehaviour_WithExpiredRefreshTokensAndOldExpiredOnes_ShouldRemoveTheRefreshTokenAndThrowException()
        {
            SetHttpContextAccessorUserForMock(claims: new());

            var validateRefreshTokenBehaviour = _fixture.Build<ValidateRefreshTokenBehaviour>().Create();

            var refreshQuery = _fixture.Build<RefreshTokenQuery>().Create();

            var nextMock = new Mock<RequestHandlerDelegate<AuthQueryResponse>>();


            var returnedRefreshToken = _fixture.Build<RefreshTokenEntity>()
                .With(x => x.Token, refreshQuery.RefreshToken)
                .With(x => x.ExpiryDateUtc, DateTime.UtcNow.AddDays(-1))
                .Create();

            var returnedOldRefreshTokens = _fixture.Build<RefreshTokenEntity>()
                .With(x => x.UserId, returnedRefreshToken.UserId)
                .With(x => x.ExpiryDateUtc, DateTime.UtcNow.AddDays(-2))
                .CreateMany(2).ToList();

            returnedOldRefreshTokens[1].ExpiryDateUtc = DateTime.UtcNow.AddDays(-3);

            List<RefreshTokenEntity> oveallReturnedRefreshTokens = [.. returnedOldRefreshTokens, returnedRefreshToken];

            SetupRefreshTokenRepositoryMock(returnedRefreshToken, oveallReturnedRefreshTokens);

            _refreshTokenRepositoryMock.Setup(x => x.RemoveRefreshToken(returnedRefreshToken));


            var handlerCall = async () => await validateRefreshTokenBehaviour.Handle(refreshQuery, nextMock.Object, default);


            var exception = handlerCall.ShouldThrow<BadRequestException>();

            _refreshTokenRepositoryMock.Verify(x => x.GetRefreshTokensByUserIdAsync(returnedRefreshToken.UserId), Times.Never);

            _refreshTokenRepositoryMock.Verify(x => x.RemoveVulnerableRefreshTokensAsync(It.Is<String>(x => x == returnedRefreshToken.Token), returnedRefreshToken.UserId), Times.Never);

            _refreshTokenRepositoryMock.Verify(x => x.RemoveRefreshToken(returnedRefreshToken), Times.Once);

            _dbContextMock.Verify(x => x.SaveChangesAsync(), Times.Once);


        }



        [Fact]
        public async Task ValidateRefreshTokenBehaviour_WithValidRefreshTokenWhenOnlyOneValidRefreshTokenExists_ShouldBypassValidation()
        {
            SetHttpContextAccessorUserForMock(claims: new());

            var validateRefreshTokenBehaviour = _fixture.Build<ValidateRefreshTokenBehaviour>().Create();

            var refreshQuery = _fixture.Build<RefreshTokenQuery>().Create();

            var nextMock = new Mock<RequestHandlerDelegate<AuthQueryResponse>>();

            var returnedRefreshToken = _fixture.Build<RefreshTokenEntity>()
                .With(x => x.Token, refreshQuery.RefreshToken)
                .With(x => x.ExpiryDateUtc, DateTime.UtcNow.AddDays(3))
                .Create();

            List<RefreshTokenEntity> oveallReturnedRefreshTokens = [ returnedRefreshToken ];

            SetupRefreshTokenRepositoryMock(returnedRefreshToken, oveallReturnedRefreshTokens);


            var response = await validateRefreshTokenBehaviour.Handle(refreshQuery, nextMock.Object, default);


            response.ShouldBeNull();

            _refreshTokenRepositoryMock.Verify(x => x.GetRefreshTokensByUserIdAsync(returnedRefreshToken.UserId), Times.Once);

            _refreshTokenRepositoryMock.Verify(x => x.RemoveRefreshTokensByUserIdAsync(returnedRefreshToken.UserId), Times.Never);

            _refreshTokenRepositoryMock.Verify(x => x.RemoveVulnerableRefreshTokensAsync(It.Is<String>(x => x == returnedRefreshToken.Token), returnedRefreshToken.UserId), Times.Never);

            _refreshTokenRepositoryMock.Verify(x => x.RemoveRefreshToken(returnedRefreshToken), Times.Never);

        }


        private void SetHttpContextAccessorUserForMock(List<Claim> claims)
        {
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            var identity = new ClaimsIdentity(claims);

            var principal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext();

            httpContext.User = principal;

            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            _fixture.Inject<IHttpContextAccessor>(httpContextAccessorMock.Object);

        }

        private void SetHttpContextAccessorRequestAndUserForMock(List<Claim> claims, string token)
        {
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            var identity = new ClaimsIdentity(claims);

            var principal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers["Authorization"] = "Bearer " + token;

            httpContext.User = principal;

            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            _fixture.Inject<IHttpContextAccessor>(httpContextAccessorMock.Object);
        }

        private void SetupRefreshTokenRepositoryMock(RefreshTokenEntity returnedRefreshToken, List<RefreshTokenEntity> oveallReturnedRefreshTokens)
        {

            _refreshTokenRepositoryMock.Setup(x => x.GetRefreshTokenByTokenAsync(returnedRefreshToken.Token)).ReturnsAsync(returnedRefreshToken);

            _refreshTokenRepositoryMock.Setup(x => x.GetRefreshTokensByUserIdAsync(returnedRefreshToken.UserId)).ReturnsAsync(oveallReturnedRefreshTokens);

            _refreshTokenRepositoryMock.Setup(x => x.RemoveRefreshTokensByUserIdAsync(returnedRefreshToken.UserId));

            _refreshTokenRepositoryMock.Setup(x => x.RemoveVulnerableRefreshTokensAsync(exceptionToken: returnedRefreshToken.Token, returnedRefreshToken.UserId));

            _dbContextMock.Setup(x => x.SaveChangesAsync());
        }
    }
}
