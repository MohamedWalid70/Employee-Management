using AutoMapper;
using Internship.EmployeeManagement.Api.Models.Auth;
using Internship.EmployeeManagement.Application.Features.Auth.Commands.Logout;
using Internship.EmployeeManagement.Application.Features.Auth.Queries.Common;
using Internship.EmployeeManagement.Application.Features.Auth.Queries.Login;
using Internship.EmployeeManagement.Application.Features.Auth.Queries.RefreshToken;
using Internship.EmployeeManagement.Common.GenericResponses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Internship.EmployeeManagement.Api.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController(IMapper mapper, ISender sender) : ControllerBase
    {
        private readonly IMapper _mapper = mapper;
        private readonly ISender _sender = sender;

        [HttpPost]
        [ProducesResponseType(typeof(GenericResult<AuthQueryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResult), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthQueryResponse>> Login([FromBody] LoginQueryParam loginQueryParam)
        {
            var loginQuery = _mapper.Map<LoginQuery>(loginQueryParam);

            var loginQueryResponseParam = await _sender.Send(loginQuery);

            var genericResult = new GenericResult<AuthQueryResponse> { StatusCode = StatusCodes.Status200OK, Data = loginQueryResponseParam };

            return Ok(genericResult);
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(GenericResult<AuthQueryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResult), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthQueryResponse>> Refresh([FromBody] string refreshToken)
        {

            var loginQueryResponseParam = await _sender.Send(new RefreshTokenQuery { RefreshToken = refreshToken } );

            var genericResult = new GenericResult<AuthQueryResponse> { StatusCode = StatusCodes.Status200OK, Data = loginQueryResponseParam };

            return Ok(genericResult);
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResult), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Logout()
        {
            await _sender.Send(new LogoutCommand());

            return Ok();
        }
    }
}
