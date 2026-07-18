using AutoFixture;
using Internship.EmployeeManagement.Application.Features.Auth.Queries.Login;
using Internship.EmployeeManagement.Core.Entities;
using Moq;
using Shouldly;

namespace Internship.EmployeeManagement.Api.UnitTests.Features.Auth
{
    public partial class AuthHandlersTests
    {
        [Fact]
        public async Task LoginHandler_WithValidCommand_ShouldReturnTheCreatedTokens()
        {
            var loginHandler = _fixture.Build<LoginQueryHandler>().Create();

            var loginQuery = _fixture.Build<LoginQuery>().Create();

            var userRoles = _fixture.Build<String>().CreateMany(2).ToList();

            var jwtToken = _fixture.Build<string>().Create();
            var refreshToken = _fixture.Build<RefreshTokenEntity>()
                .With(x => x.UserId, loginQuery.SharedUser.Id)
                .Create();


            _userManagerMock.Setup(x => x.GetRolesAsync(loginQuery.SharedUser)).ReturnsAsync(userRoles);

            _tokenGeneratorMock.Setup(x => x.GenerateJwtToken(loginQuery.SharedUser, userRoles)).Returns(jwtToken);

            _tokenGeneratorMock.Setup(x => x.GenerateRefreshToken(loginQuery.SharedUser.Id)).Returns(refreshToken);

            _dbContextMock.Setup(x => x.SaveChangesAsync());

            _refreshTokenRepositoryMock.Setup(x => x.AddRefreshTokenAsync(refreshToken));


            var result = await loginHandler.Handle(loginQuery, default);

            result.AccessToken.ShouldBe(jwtToken);

            result.RefreshToken.ShouldBe(refreshToken.Token);

            _userManagerMock.Verify(x => x.GetRolesAsync(It.Is<UserEntity>(x => x.Id == loginQuery.SharedUser.Id)), Times.Once);

            _tokenGeneratorMock.Verify(x => x.GenerateJwtToken(It.Is<UserEntity>(x => x.Id == loginQuery.SharedUser.Id), userRoles), Times.Once);

            _tokenGeneratorMock.Verify(x => x.GenerateRefreshToken(It.Is<int>(x => x == loginQuery.SharedUser.Id)), Times.Once);

            _refreshTokenRepositoryMock.Verify(x => x.AddRefreshTokenAsync(refreshToken), Times.Once);

            _dbContextMock.Verify(x => x.SaveChangesAsync(), Times.Once);

        }

    }
}
