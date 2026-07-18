using Internship.EmployeeManagement.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Internship.EmployeeManagement.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository, IHttpContextAccessor httpContextAccessor) : IRequestHandler<LogoutCommand, Unit>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if(int.TryParse(userIdClaim?.Value, out int userId))
                await _refreshTokenRepository.RemoveRefreshTokensByUserIdAsync(userId);
            

            return Unit.Value;
        }
    }
}
