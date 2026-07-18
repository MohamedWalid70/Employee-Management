using AutoFixture;
using Internship.EmployeeManagement.Application.Exceptions;
using Internship.EmployeeManagement.Application.Features.Employees.Commands;
using Internship.EmployeeManagement.Application.Features.Employees.Commands.DeleteEmployee;
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
        public async Task ValidateEmployeeAsync_WithValidUpdateEmployeeQuery_ValidationSucceedsAndAssignsTheTargetEmployee()
        {
            var validateEmployeeBehavoiur = _fixture.Build<ValidateEmployeeExistenceBehavoiur<UpdateEmployeeCommand, Unit>>().Create();

            var validEmployeeId = Guid.NewGuid();

            var employee = _fixture.Build<EmployeeEntity>().With(x => x.Id, validEmployeeId).Create();
            var updateEmployeeCommand = _fixture.Build<UpdateEmployeeCommand>()
                  .With(x => x.EntityId, validEmployeeId)
                  .Create();

            var nextMock = new Mock<RequestHandlerDelegate<Unit>>();

            _writeRepoMock.Setup(x => x.GetEmployeeByIdAsync(employee.Id)).
                ReturnsAsync(employee);

            var result = await validateEmployeeBehavoiur.Handle(updateEmployeeCommand, nextMock.Object, default);

            result.ShouldBe(Unit.Value);
            updateEmployeeCommand.SharedObject.ShouldBe(employee);

            _writeRepoMock.Verify(x => x.GetEmployeeByIdAsync(validEmployeeId), Times.Once);

        }

        [Fact]
        public async Task ValidateEmployeeAsync_WithInvalidUpdateEmployeeQuery_ThrowsNotFoundException()
        {
            var validateEmployeeBehavoiur = _fixture.Build<ValidateEmployeeExistenceBehavoiur<UpdateEmployeeCommand, Unit>>().Create();

            var validEmployeeId = Guid.NewGuid();

            var invalidId = Guid.NewGuid();

            var employee = _fixture.Build<EmployeeEntity>().With(x => x.Id, validEmployeeId).Create();
            var updateEmployeeCommand = _fixture.Build<UpdateEmployeeCommand>()
                  .With(x => x.EntityId, invalidId)
                  .Create();

            var nextMock = new Mock<RequestHandlerDelegate<Unit>>();

            _writeRepoMock.Setup(x => x.GetEmployeeByIdAsync(employee.Id)).
                ReturnsAsync(employee);

            var handlerCall = async () => await validateEmployeeBehavoiur.Handle(updateEmployeeCommand, nextMock.Object, default);

            handlerCall.ShouldThrow<NotFoundException>();

            _writeRepoMock.Verify(x => x.GetEmployeeByIdAsync(invalidId), Times.Once);

        }



        [Fact]
        public async Task ValidateEmployeeAsync_WithValidDeleteEmployeeQuery_ValidationSucceedsAndAssignsTheTargetEmployee()
        {
            var validateEmployeeBehavoiur = _fixture.Build<ValidateEmployeeExistenceBehavoiur<DeleteEmployeeCommand, Unit>>().Create();

            var validEmployeeId = Guid.NewGuid();

            var employee = _fixture.Build<EmployeeEntity>().With(x => x.Id, validEmployeeId).Create();
            var deleteEmployeeCommand = _fixture.Build<DeleteEmployeeCommand>()
                  .With(x => x.EntityId, validEmployeeId)
                  .Create();

            var nextMock = new Mock<RequestHandlerDelegate<Unit>>();

            _writeRepoMock.Setup(x => x.GetEmployeeByIdAsync(employee.Id)).
                ReturnsAsync(employee);

            var result = await validateEmployeeBehavoiur.Handle(deleteEmployeeCommand, nextMock.Object, default);

            result.ShouldBe(Unit.Value);
            deleteEmployeeCommand.SharedObject.ShouldBe(employee);

            _writeRepoMock.Verify(x => x.GetEmployeeByIdAsync(validEmployeeId), Times.Once);

        }

        [Fact]
        public async Task ValidateEmployeeAsync_WithInvalidDeleteEmployeeQuery_ThrowsNotFoundException()
        {
            var validateEmployeeBehavoiur = _fixture.Build<ValidateEmployeeExistenceBehavoiur<DeleteEmployeeCommand, Unit>>().Create();

            var validEmployeeId = Guid.NewGuid();

            var invalidId = Guid.NewGuid();

            var employee = _fixture.Build<EmployeeEntity>().With(x => x.Id, validEmployeeId).Create();
            var deleteEmployeeCommand = _fixture.Build<DeleteEmployeeCommand>()
                  .With(x => x.EntityId, invalidId)
                  .Create();

            var nextMock = new Mock<RequestHandlerDelegate<Unit>>();

            _writeRepoMock.Setup(x => x.GetEmployeeByIdAsync(employee.Id)).
                ReturnsAsync(employee);

            var handlerCall = async () => await validateEmployeeBehavoiur.Handle(deleteEmployeeCommand, nextMock.Object, default);

            handlerCall.ShouldThrow<NotFoundException>();

            _writeRepoMock.Verify(x => x.GetEmployeeByIdAsync(invalidId), Times.Once);

        }
    }

}
