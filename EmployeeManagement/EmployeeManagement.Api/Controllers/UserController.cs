using AutoMapper;
using Internship.EmployeeManagement.Api.Models.Users;
using Internship.EmployeeManagement.Application.Features.Common;
using Internship.EmployeeManagement.Application.Features.Users.Commands.CreateUser;
using Internship.EmployeeManagement.Application.Features.Users.Queries.GetUserById;
using Internship.EmployeeManagement.Common.GenericResponses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Internship.EmployeeManagement.Api.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UserController(IMapper mapper, ISender sender) : ControllerBase
    {
        private readonly IMapper _mapper = mapper;
        private readonly ISender _sender = sender;

        [HttpPost]
        [ProducesResponseType(typeof(IdResponse<int>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(GenericResult), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IdResponse<int>>> CreateUser([FromBody] CreateUserCommandParam createUserCommandParam)
        {
            var createUserCommand = _mapper.Map<CreateUserCommand>(createUserCommandParam);

            var newlyCreatedId = await _sender.Send(createUserCommand);

            return CreatedAtAction(nameof(GetUserById), new { id = newlyCreatedId.Id }, newlyCreatedId);

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(GenericResult<GetUserByIdQueryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<GenericResult<GetUserByIdQueryResponse>>> GetUserById(int id)
        {
            var getUserByIdQueryParam = await _sender.Send(new GetUserByIdQuery(id));

            var result = new GenericResult<GetUserByIdQueryResponse> { StatusCode = StatusCodes.Status200OK, Data = getUserByIdQueryParam };

            return Ok(result);
        }
    }
}
