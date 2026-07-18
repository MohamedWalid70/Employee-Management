using AutoFixture;
using Castle.Components.DictionaryAdapter.Xml;
using Internship.EmployeeManagement.Application.Features.Employees.Queries.GetEmployeeById;
using Internship.EmployeeManagement.Application.Features.Employees.Queries.GetEmployeeHistoryByEmployeeId;
using Internship.EmployeeManagement.Application.Features.Employees.Queries.GetPaginatedEmployees;
using Internship.EmployeeManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using Shouldly;
using System.Net.Sockets;
using System.Xml;

namespace Internship.EmployeeManagement.Api.UnitTests.Features.Employees
{
    public partial class EmployeeHandlerTests
    {
        [Theory]
        [InlineData(1, 2)]
        public async Task GetPaginatedEmployeesHandler_WithValidInput_ReturnsEmployees(int pageNumber, int pageSize)
        {
            var getPaginatedEmployeesHandler = _fixture.Build<GetPaginatedEmployeesQueryHandler>().Create();

            var getPaginatedEmployeesQuery = new GetPaginatedEmployeesQuery(pageNumber, pageSize);

            var employeeEntities = _fixture.Build<EmployeeEntity>().CreateMany(2).ToList();
            var employeeModels = _fixture.Build<GetEmployeeByIdQueryResponse>().CreateMany(2).ToList();

            _readRepoMock.Setup(x => x.GetPaginatedEmployeesAsync(pageNumber - 1, pageSize))
                .Returns(employeeEntities.ToAsyncEnumerable());

            _mapperMock.Setup(x => x.Map<GetEmployeeByIdQueryResponse>(employeeEntities[0])).Returns(employeeModels[0]);
            _mapperMock.Setup(x => x.Map<GetEmployeeByIdQueryResponse>(employeeEntities[1])).Returns(employeeModels[1]);

            var result = getPaginatedEmployeesHandler.Handle(getPaginatedEmployeesQuery, default);

            var count = await result.CountAsync();

            count.ShouldBe(2);

            _readRepoMock.Verify(x => x.GetPaginatedEmployeesAsync(pageNumber - 1, pageSize), Times.Once);
        }


        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(0, 2)]
        [InlineData(-1, -3)]
        public async Task GetPaginatedEmployeesHandler_WithInvalidInput_ReturnsEmptyList(int pageNumber, int pageSize)
        {
            var getPaginatedEmployeesHandler = _fixture.Build<GetPaginatedEmployeesQueryHandler>().Create();

            var getPaginatedEmployeesQuery = new GetPaginatedEmployeesQuery(pageNumber, pageSize);

            var employeeEntities = _fixture.Build<EmployeeEntity>().CreateMany(2).ToList();
            var employeeModels = _fixture.Build<GetEmployeeByIdQueryResponse>().CreateMany(2).ToList();

            _readRepoMock.Setup(x => x.GetPaginatedEmployeesAsync(pageNumber - 1, pageSize))
                .Returns(employeeEntities.ToAsyncEnumerable());

            _mapperMock.Setup(x => x.Map<GetEmployeeByIdQueryResponse>(employeeEntities[0])).Returns(employeeModels[0]);
            _mapperMock.Setup(x => x.Map<GetEmployeeByIdQueryResponse>(employeeEntities[1])).Returns(employeeModels[1]);

            var result = getPaginatedEmployeesHandler.Handle(getPaginatedEmployeesQuery, default);

            var count = await result.CountAsync();

            count.ShouldBe(0);

            _readRepoMock.Verify(x => x.GetPaginatedEmployeesAsync(pageNumber - 1, pageSize), Times.Never);
        }

        [Fact]
        public async Task GetEmployeeHistoryByIdHandler_WithValidId_ReturnsTheRespectiveEmployeeHistory()
        {
            var validEmployeeId = Guid.NewGuid();

            var getEmployeeHistoryByIdQuery = new GetEmployeeHistoryByIdQuery(validEmployeeId);

            var getEmployeeHistoryByIdHandler = _fixture.Build<GetEmployeeHistoryByIdQueryHandler>().Create();

            var historyList = _fixture.Build<EmployeeHistoryEntity>().CreateMany(4);

            var mappedHistory = _fixture.Build<GetEmployeeHistoryByIdQueryResponse>().CreateMany(4);

            _employeeHistory.Setup(x => x.GetEmployeeHistoryByEmployeeIdAsync(validEmployeeId)).
                ReturnsAsync(historyList);

            _mapperMock.Setup(x => x.Map<IEnumerable<GetEmployeeHistoryByIdQueryResponse>>(historyList)).Returns(mappedHistory);

            var result = await getEmployeeHistoryByIdHandler.Handle(getEmployeeHistoryByIdQuery, default);

            result?.Count().ShouldBe(4);

            _employeeHistory.Verify(x => x.GetEmployeeHistoryByEmployeeIdAsync(validEmployeeId), Times.Once);

            _mapperMock.Verify(x => x.Map<IEnumerable<GetEmployeeHistoryByIdQueryResponse>>(historyList), Times.Once);

        }


        [Fact]
        public async Task GetEmployeeHistoryByIdHandler_WithInvalidId_ShouldReturnEmptyList()
        {
            var validEmployeeId = Guid.NewGuid();

            var invalidId = Guid.NewGuid();

            var getEmployeeHistoryByIdQuery = new GetEmployeeHistoryByIdQuery(invalidId);

            var getEmployeeHistoryByIdHandler = _fixture.Build<GetEmployeeHistoryByIdQueryHandler>().Create();

            var historyList = _fixture.Build<EmployeeHistoryEntity>().CreateMany(4);

            var mappedHistory = _fixture.Build<GetEmployeeHistoryByIdQueryResponse>().CreateMany(4);

            _employeeHistory.Setup(x => x.GetEmployeeHistoryByEmployeeIdAsync(validEmployeeId)).
                ReturnsAsync(historyList);

            _mapperMock.Setup(x => x.Map<IEnumerable<GetEmployeeHistoryByIdQueryResponse>>(historyList)).Returns(mappedHistory);

            var result = await getEmployeeHistoryByIdHandler.Handle(getEmployeeHistoryByIdQuery, default);

            result.ShouldBeEmpty();

            _employeeHistory.Verify(x => x.GetEmployeeHistoryByEmployeeIdAsync(invalidId), Times.Once);

            _mapperMock.Verify(x => x.Map<IEnumerable<GetEmployeeHistoryByIdQueryResponse>>(historyList), Times.Never);

        }

        [Fact]
        public async Task GetEmployeeByIdHandler_WithValidId_ReturnsTheEmployee()
        {
            var validEmployeeId = Guid.NewGuid();

            var getEmployeeByIdHandler = _fixture.Build<GetEmployeeByIdHandler>().Create();

            var getEmployeeByIdQuery = new GetEmployeeByIdQuery(validEmployeeId);

            var employee = _fixture.Build<EmployeeEntity>().With(x => x.Id, validEmployeeId).Create();
            var getEmployeeQueryParam = _fixture.Build<GetEmployeeByIdQueryResponse>().With(x => x.Id, employee.Id).Create();

            _readRepoMock.Setup(x => x.GetEmployeeByIdAsync(validEmployeeId)).
                ReturnsAsync(employee);

            _mapperMock.Setup(x => x.Map<GetEmployeeByIdQueryResponse>(employee)).Returns(getEmployeeQueryParam);

            var result = await getEmployeeByIdHandler.Handle(getEmployeeByIdQuery, default);

            result.ShouldNotBeNull();
            result.Id.ShouldBe(validEmployeeId);

            _readRepoMock.Verify(x => x.GetEmployeeByIdAsync(validEmployeeId), Times.Once);

        }

        [Fact]
        public async Task GetEmployeeByIdHandler_WithInvalidId_ShouldReturnNull()
        {
            var validEmployeeId = Guid.NewGuid();

            var invalidId = Guid.NewGuid();

            var getEmployeeByIdHandler = _fixture.Build<GetEmployeeByIdHandler>().Create();

            var getEmployeeByIdQuery = new GetEmployeeByIdQuery(invalidId);

            var employee = _fixture.Build<EmployeeEntity>().With(x => x.Id, validEmployeeId).Create();
            var getEmployeeQueryParam = _fixture.Build<GetEmployeeByIdQueryResponse>().With(x => x.Id, employee.Id).Create();

            _readRepoMock.Setup(x => x.GetEmployeeByIdAsync(validEmployeeId)).
                ReturnsAsync(employee);

            _mapperMock.Setup(x => x.Map<GetEmployeeByIdQueryResponse>(employee)).Returns(getEmployeeQueryParam);

            var response = await getEmployeeByIdHandler.Handle(getEmployeeByIdQuery, default);

            response.ShouldBeNull();

            _readRepoMock.Verify(x => x.GetEmployeeByIdAsync(invalidId), Times.Once);

        }
    }
}
