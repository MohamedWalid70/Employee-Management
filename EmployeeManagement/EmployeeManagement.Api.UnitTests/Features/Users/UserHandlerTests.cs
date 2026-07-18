using AutoFixture;
using AutoMapper;
using Internship.EmployeeManagement.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Internship.UserManagement.Api.UnitTests.Features.Users
{
    public partial class UserHandlersTests
    {
        private readonly Mock<UserManager<UserEntity>> _userManagerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Fixture _fixture;
        public UserHandlersTests()
        {
            var userStoreMock = new Mock<IUserStore<UserEntity>>();
            _userManagerMock = new Mock<UserManager<UserEntity>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            _mapperMock = new Mock<IMapper>();
            _fixture = new();
            _fixture.Inject<IMapper>(_mapperMock.Object);
            _fixture.Inject<UserManager<UserEntity>>(_userManagerMock.Object);
        }
    }
}
