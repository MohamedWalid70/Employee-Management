using AutoFixture;
using Internship.EmployeeManagement.Application.Features.Users.Queries.GetUserById;
using Internship.EmployeeManagement.Core.Entities;
using MockQueryable;
using Moq;
using Shouldly;

namespace Internship.UserManagement.Api.UnitTests.Features.Users
{
    public partial class UserHandlersTests
    {
        [Theory]
        [InlineData(1)]
        public async Task GetUserByIdHandler_WithValidId_ShouldReturnTheUser(int id)
        {
            var getUserByIdHandler = _fixture.Build<GetUserByIdQueryHandler>().Create();

            var getUserByIdQuery = new GetUserByIdQuery(id);

            var users = _fixture.Build<UserEntity>().CreateMany(4).ToList();

            users[0].Id = id;

            var data = users.BuildMock();

            var getUserQueryParam = _fixture.Build<GetUserByIdQueryResponse>().With(x => x.Id, users[0].Id).Create();

            _userManagerMock.Setup(x => x.Users).Returns(data);

            _mapperMock.Setup(x => x.Map<GetUserByIdQueryResponse>(users[0])).Returns(getUserQueryParam);

            var result = await getUserByIdHandler.Handle(getUserByIdQuery, default);

            result.ShouldNotBeNull();
            result.Id.ShouldBe(id);

            _userManagerMock.Verify(x => x.Users, Times.Once);
            _mapperMock.Verify(x => x.Map<GetUserByIdQueryResponse>(It.Is<UserEntity>(u => u.Id == users[0].Id)), Times.Once);

        }

        [Theory]
        [InlineData(9999999)]
        public async Task GetUserByIdHandler_WithInvalidId_ShouldReturnNull(int id)
        {
            var getUserByIdHandler = _fixture.Build<GetUserByIdQueryHandler>().Create();

            var getUserByIdQuery = new GetUserByIdQuery(id);

            var users = _fixture.Build<UserEntity>().CreateMany(4).ToList();

            var data = users.BuildMock();

            var getUserQueryParam = _fixture.Build<GetUserByIdQueryResponse>().With(x => x.Id, users[0].Id).Create();

            _userManagerMock.Setup(x => x.Users).Returns(data);

            _mapperMock.Setup(x => x.Map<GetUserByIdQueryResponse>(users[0])).Returns(getUserQueryParam);


            var response = await getUserByIdHandler.Handle(getUserByIdQuery, default);


            response.ShouldBeNull();

            _mapperMock.Verify(x => x.Map<GetUserByIdQueryResponse>(null), Times.Once);


        }
    }
}
