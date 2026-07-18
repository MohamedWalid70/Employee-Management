using AutoFixture;
using AutoMapper;
using Internship.EmployeeManagement.Application;
using Internship.EmployeeManagement.Core.Interfaces;
using Internship.EmployeeManagement.Core.Interfaces.Employee;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Internship.EmployeeManagement.Api.UnitTests.Features.Employees
{
    public partial class EmployeeHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Fixture _fixture;
        private readonly Mock<IReadDbContext> _readDbContextMock;
        private readonly Mock<IEventBus> _eventBusMock;
        private readonly Mock<IWriteDbContext> _writeDbContextMock;
        private readonly Mock<IEmployeeHistory> _employeeHistory;
        private readonly Mock<IEmployeeRepository<IWriteDbContext>> _writeRepoMock;
        private readonly Mock<IEmployeeRepository<IReadDbContext>> _readRepoMock;
        public EmployeeHandlerTests()
        {
            _readRepoMock = new();
            _writeRepoMock = new();
            _employeeHistory = new();
            _mapperMock = new();
            _readDbContextMock = new();
            _writeDbContextMock = new();
            _eventBusMock = new();
            _fixture = new();
            _fixture.Inject(_mapperMock.Object);
            _fixture.Inject(_writeDbContextMock.Object);
            _fixture.Inject(_readDbContextMock.Object);
            _fixture.Inject(_eventBusMock.Object);
            _fixture.Inject(_employeeHistory.Object);
            _fixture.Inject(_writeRepoMock.Object);
            _fixture.Inject(_readRepoMock.Object);
        }

    }

}
