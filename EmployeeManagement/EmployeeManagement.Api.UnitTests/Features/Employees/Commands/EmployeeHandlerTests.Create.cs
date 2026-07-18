using AutoFixture;
using Internship.EmployeeManagement.Application.Features.Employees.Commands.CreateEmployee;
using Internship.EmployeeManagement.Core.Entities;
using Moq;
using Shouldly;

namespace Internship.EmployeeManagement.Api.UnitTests.Features.Employees
{
    public partial class EmployeeHandlerTests
    {

        [Fact]
        public async Task CreateEmployeeHandler_WithValidCommand_ReturnsTheCreatedEmployeeId()
        {
            var createEmployeeHandler = _fixture.Build<CreateEmployeeCommandHandler>().Create();

            var employeeToBeCreated = _fixture.Build<CreateEmployeeCommand>().Create();
            EmployeeEntity employeeEntity = _fixture.Build<EmployeeEntity>().With(x => x.Name, employeeToBeCreated.Name).Create();

            var employeeCreatedEvent = _fixture.Build<EmployeeCreatedEvent>().With(x => x.Id, employeeEntity.Id).With(x => x.Name, employeeToBeCreated.Name).Create();

            SetupCreateEmployeeHandlerTests(employeeEntity, employeeCreatedEvent, employeeToBeCreated);

            var result = await createEmployeeHandler.Handle(employeeToBeCreated, default);

            result.Id.ShouldBe(employeeEntity.Id);

            _writeRepoMock.Verify(x => x.AddEmployeeAsync(employeeEntity), Times.Once);

            _writeDbContextMock.Verify(x => x.SaveChangesAsync(), Times.Once);

            _eventBusMock.Verify(x => x.PublishAsync(It.Is<EmployeeCreatedEvent>(x => x.Id == employeeCreatedEvent.Id), default), Times.Once);

        }

        [Fact]
        public async Task CreateEmployeeHandler_WithMissingAgeParameter_ReturnsTheCreatedEmployeeId()
        {
            var createEmployeeHandler = _fixture.Build<CreateEmployeeCommandHandler>().Create();

            var createdGuid = Guid.NewGuid();

            var employeeToBeCreated = _fixture.Build<CreateEmployeeCommand>().Without(x => x.Age).Create();
            EmployeeEntity employeeEntity = _fixture.Build<EmployeeEntity>().With(x => x.Age, employeeToBeCreated.Age).With(x => x.Id, createdGuid).Create();

            var employeeCreatedEvent = _fixture.Build<EmployeeCreatedEvent>().With(x => x.Id, employeeEntity.Id).With(x => x.Age, employeeToBeCreated.Age).Create();

            SetupCreateEmployeeHandlerTests(employeeEntity, employeeCreatedEvent, employeeToBeCreated);

            var result = await createEmployeeHandler.Handle(employeeToBeCreated, default);

            result.Id.ShouldBe(createdGuid);

            _writeRepoMock.Verify(x => x.AddEmployeeAsync(It.Is<EmployeeEntity>(x => x.Age == employeeToBeCreated.Age)));

            _writeDbContextMock.Verify(x => x.SaveChangesAsync(), Times.Once);

            _eventBusMock.Verify(x => x.PublishAsync(employeeCreatedEvent, default), Times.Once);

        }

        private void SetupCreateEmployeeHandlerTests(EmployeeEntity employeeEntity, EmployeeCreatedEvent employeeCreatedEvent, CreateEmployeeCommand createEmployeeCommand)
        {
            _mapperMock.Setup(x => x.Map<EmployeeEntity>(createEmployeeCommand)).Returns(employeeEntity);

            _writeRepoMock.Setup(x => x.AddEmployeeAsync(employeeEntity));

            _mapperMock.Setup(x => x.Map<EmployeeCreatedEvent>(employeeEntity)).Returns(employeeCreatedEvent);

            _eventBusMock.Setup(x => x.PublishAsync(employeeCreatedEvent, default));
        }

    }

}
