using AutoMapper;
using Internship.EmployeeManagement.Api.Models.Employees;
using Internship.EmployeeManagement.Application.Features.Common;
using Internship.EmployeeManagement.Application.Features.Employees.Commands.CreateEmployee;
using Internship.EmployeeManagement.Application.Features.Employees.Commands.DeleteEmployee;
using Internship.EmployeeManagement.Application.Features.Employees.Commands.UpdateEmployee;
using Internship.EmployeeManagement.Application.Features.Employees.Queries.GetEmployeeById;
using Internship.EmployeeManagement.Application.Features.Employees.Queries.GetEmployeeHistoryByEmployeeId;
using Internship.EmployeeManagement.Application.Features.Employees.Queries.GetPaginatedEmployees;
using Internship.EmployeeManagement.Common.GenericResponses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Internship.EmployeeManagement.Api.Controllers
{
    [Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Route("api/v1/employees")]
    [ApiController]
    public class EmployeesController(ISender sender, IMapper mapper) : ControllerBase
    {
        private readonly ISender _sender = sender;
        private readonly IMapper _mapper = mapper;

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IAsyncEnumerable<GetEmployeeByIdQueryResponse> GetPaginatedEmployees(int pageNumber = 1, int pageSize = 5)
        {
            return _sender.CreateStream(new GetPaginatedEmployeesQuery(pageNumber, pageSize));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(GenericResult<GetEmployeeByIdQueryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResult), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GenericResult<GetEmployeeByIdQueryResponse>>> GetEmployeeDataById(Guid id)
        {
            var employeeData = await _sender.Send(new GetEmployeeByIdQuery(id));

            var result = new GenericResult<GetEmployeeByIdQueryResponse> { StatusCode = StatusCodes.Status200OK, Data = employeeData };

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("history/{id:guid}")]
        [ProducesResponseType(typeof(GenericResult<IEnumerable<GetEmployeeHistoryByIdQueryResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<GenericResult<IEnumerable<GetEmployeeHistoryByIdQueryResponse>>>> GetRelatedEmployeeHistoryByEmployeeId(Guid id)
        {
            var employeeHistory = await _sender.Send(new GetEmployeeHistoryByIdQuery(id));
            var result = new GenericResult<IEnumerable<GetEmployeeHistoryByIdQueryResponse>> { StatusCode = StatusCodes.Status200OK, Data = employeeHistory };
            return Ok(result);

        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(IdResponse<Guid>), StatusCodes.Status201Created)]
        public async Task<ActionResult<IdResponse<Guid>>> CreateEmployee([FromBody] CreateEmployeeCommandParam createEmployeeCommandParam)
        {
            var createEmployeeCommand = _mapper.Map<CreateEmployeeCommand>(createEmployeeCommandParam);

            var newlyCreatedId = await _sender.Send(createEmployeeCommand);

            return CreatedAtAction(nameof(GetEmployeeDataById), new {id = newlyCreatedId.Id}, newlyCreatedId);
        
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GenericResult), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateEmployee([FromBody] UpdateEmployeeCommandParam updateEmployeeCommandParam)
        {
            var updateEmployeeCommand = _mapper.Map<UpdateEmployeeCommand>(updateEmployeeCommandParam);

            await _sender.Send(updateEmployeeCommand); 

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(GenericResult), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteEmployee(Guid id)
        {
            await _sender.Send(new DeleteEmployeeCommand { EntityId = id });
            return NoContent();

        }
    }
}
