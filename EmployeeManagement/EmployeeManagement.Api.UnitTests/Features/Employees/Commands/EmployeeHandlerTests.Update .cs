using AutoFixture;
using Internship.EmployeeManagement.Application.Features.Employees.Commands.UpdateEmployee;
using Internship.EmployeeManagement.Core.Entities;
using MediatR;
using Moq;
using Shouldly;

namespace Internship.EmployeeManagement.Api.UnitTests.Features.Employees
{
    public partial class EmployeeHandlerTests
    {
        [Fact]
        public async Task UpdateEmployeeHandler_WithValidInput_ReturnsUnit()
        {
            var originalEmployeeData = _fixture.Build<EmployeeEntity>().Create();
            var updateEmployeeCommand = _fixture.Build<UpdateEmployeeCommand>()
                .With(x => x.EntityId, originalEmployeeData.Id)
                .With(x => x.SharedObject, originalEmployeeData)
                .Create();

            var updateEmployeeHandler = _fixture.Build<UpdateEmployeeCommandHandler>().Create();

            var employeeUpdatedEvent = _fixture.Build<EmployeeUpdatedEvent>()
                .With(x => x.Id, originalEmployeeData.Id)
                .With(x => x.Name, updateEmployeeCommand.Name)
                .Create();

            _writeDbContextMock.Setup(x => x.SaveChangesAsync());

            _mapperMock.Setup(x => x.Map(updateEmployeeCommand, originalEmployeeData))
                .Callback(() => { 
                    originalEmployeeData.Name = updateEmployeeCommand.Name;
                    originalEmployeeData.Title = updateEmployeeCommand.Title;
                    originalEmployeeData.Age = updateEmployeeCommand.Age;
                });

            _mapperMock.Setup(x => x.Map<EmployeeUpdatedEvent>(updateEmployeeCommand.SharedObject)).Returns(employeeUpdatedEvent);

            _eventBusMock.Setup(x => x.PublishAsync(employeeUpdatedEvent, default));
            
            var result = await updateEmployeeHandler.Handle(updateEmployeeCommand, default);

            result.ShouldBe(Unit.Value);

            _writeDbContextMock.Verify(x => x.SaveChangesAsync(), Times.Once);

            _mapperMock.Verify(x => x.Map(updateEmployeeCommand, originalEmployeeData), Times.Once);

            _mapperMock.Verify(x => x.Map<EmployeeUpdatedEvent>(It.Is<EmployeeEntity>(x => x.Title == updateEmployeeCommand.Title)), Times.Once);

            _eventBusMock.Verify(x => x.PublishAsync(employeeUpdatedEvent, default), Times.Once);

        }

    }
}
