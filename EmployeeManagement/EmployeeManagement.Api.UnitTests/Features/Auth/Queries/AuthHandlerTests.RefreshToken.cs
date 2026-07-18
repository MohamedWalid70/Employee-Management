using AutoFixture;
using Internship.EmployeeManagement.Application.Features.Auth.Queries.RefreshToken;
using Internship.EmployeeManagement.Core.Entities;
using Moq;
using Shouldly;

namespace Internship.EmployeeManagement.Api.UnitTests.Features.Auth
{
    public partial class AuthHandlersTests
    {
        [Fact]
        public async Task RefreshTokenHandler_WithValidCommand_ShouldReturnTheCreatedTokens()
        {
            var refreshHandler = _fixture.Build<RefreshTokenQueryHandler>().Create();

            var refreshQuery = _fixture.Build<RefreshTokenQuery>().Create();

            var userRoles = _fixture.Build<String>().CreateMany(2).ToList();

            var jwtToken = _fixture.Build<string>().Create();
            var refreshToken = _fixture.Build<RefreshTokenEntity>()
                .With(x => x.UserId, refreshQuery.SharedExistentToken.UserId)
                .Create();


            _userManagerMock.Setup(x => x.GetRolesAsync(refreshQuery.SharedExistentToken.User)).ReturnsAsync(userRoles);

            _tokenGeneratorMock.Setup(x => x.GenerateJwtToken(refreshQuery.SharedExistentToken.User, userRoles)).Returns(jwtToken);

            _tokenGeneratorMock.Setup(x => x.GenerateRefreshToken(refreshQuery.SharedExistentToken.User.Id)).Returns(refreshToken);

            _dbContextMock.Setup(x => x.SaveChangesAsync());


            var result = await refreshHandler.Handle(refreshQuery, default);

            result.AccessToken.ShouldBe(jwtToken);

            result.RefreshToken.ShouldBe(refreshToken.Token);

            _userManagerMock.Verify(x => x.GetRolesAsync(It.Is<UserEntity>(x => x.Id == refreshQuery.SharedExistentToken.User.Id)), Times.Once);

            _tokenGeneratorMock.Verify(x => x.GenerateJwtToken(It.Is<UserEntity>(x => x.Id == refreshQuery.SharedExistentToken.User.Id), userRoles), Times.Once);

            _tokenGeneratorMock.Verify(x => x.GenerateRefreshToken(It.Is<int>(x => x == refreshQuery.SharedExistentToken.User.Id)), Times.Once);

            _dbContextMock.Verify(x => x.SaveChangesAsync(), Times.Once);

        }

    }
}
