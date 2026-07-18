using AutoFixture;
using Internship.EmployeeManagement.Application.Tokens;
using Internship.EmployeeManagement.Core.Entities;
using Internship.EmployeeManagement.Core.Interfaces;
using Internship.EmployeeManagement.Infrastructure.Presistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;

namespace Internship.EmployeeManagement.Api.UnitTests.Features.Auth
{
    public partial class AuthHandlersTests
    {
        private readonly Mock<UserManager<UserEntity>> _userManagerMock;
        private readonly Fixture _fixture;
        private readonly Mock<ITokenGenerator> _tokenGeneratorMock;
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private readonly Mock<IOptions<JwtSettings>> _jwtSettingsMock;
        private readonly Mock<IReadDbContext> _dbContextMock;
        public AuthHandlersTests()
        {
            var userStoreMock = new Mock<IUserStore<UserEntity>>();
            _userManagerMock = new Mock<UserManager<UserEntity>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _refreshTokenRepositoryMock = new();
            _fixture = new();
            _jwtSettingsMock = new();
            _tokenGeneratorMock = new();
            _dbContextMock = new();

            _fixture.Inject<UserManager<UserEntity>>(_userManagerMock.Object);
            _fixture.Inject<ITokenGenerator>(_tokenGeneratorMock.Object);
            _fixture.Inject<IRefreshTokenRepository>(_refreshTokenRepositoryMock.Object);
            _fixture.Inject<IReadDbContext>(_dbContextMock.Object);
        }

    }
}
