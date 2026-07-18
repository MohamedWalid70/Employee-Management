using AutoFixture;
using Internship.EmployeeManagement.Application.Features.Employees.Commands.DeleteEmployee;
using Internship.EmployeeManagement.Core.Entities;
using MediatR;
using Moq;
using Shouldly;

namespace Internship.EmployeeManagement.Api.UnitTests.Features.Employees
{
    public partial class EmployeeHandlerTests
    {
        [Fact]
        public async Task DeleteEmployeeHandler_WithValidId_ReturnsUnit()
        {
            var deleteEmployeeHandler = _fixture.Build<DeleteEmployeeCommandHandler>().Create();

            var validExistentGuid = Guid.NewGuid();

            var returnedEmployee = _fixture.Build<EmployeeEntity>().With(x => x.Id, validExistentGuid).Create();

            var deleteEmployeCommand = _fixture.Build<DeleteEmployeeCommand>()
                .With(x => x.SharedObject, returnedEmployee)
                .With(x => x.EntityId, validExistentGuid).Create();

            var employeeDeleteEvent = _fixture.Build<EmployeeDeleteEvent>().With(x => x.Id, validExistentGuid).Create();

            _writeRepoMock.Setup(x => x.RemoveEmployee(returnedEmployee));

            _eventBusMock.Setup(x => x.PublishAsync(employeeDeleteEvent, default));

            var result = await deleteEmployeeHandler.Handle(deleteEmployeCommand, default);

            result.ShouldBe(Unit.Value);

            _writeDbContextMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            _writeRepoMock.Verify(x => x.RemoveEmployee(returnedEmployee), Times.Once);

            _eventBusMock.Verify(x => x.PublishAsync(employeeDeleteEvent, default), Times.Once);

        }
    }
}
