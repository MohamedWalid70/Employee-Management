using Internship.EmployeeManagement.Application.Features.Auth.Queries.Common;
using Internship.EmployeeManagement.Core.Entities;
using Internship.EmployeeManagement.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Internship.EmployeeManagement.Application.Features.Auth.Queries.RefreshToken
{
    public class RefreshTokenQueryHandler(ITokenGenerator tokenGenerator, UserManager<UserEntity> userManager, IReadDbContext dbContext) : IRequestHandler<RefreshTokenQuery, AuthQueryResponse>
    {
        private readonly ITokenGenerator _tokenGenerator = tokenGenerator;
        private readonly UserManager<UserEntity> _userManager = userManager;
        private readonly IReadDbContext _dbContext = dbContext;

        public async Task<AuthQueryResponse> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
        {
            var userRoles = await _userManager.GetRolesAsync(request.SharedExistentToken.User);

            var accessToken = _tokenGenerator.GenerateJwtToken(request.SharedExistentToken.User, userRoles);

            var refreshToken = _tokenGenerator.GenerateRefreshToken(request.SharedExistentToken.User.Id);

            await SaveRefreshTokensAsync(refreshToken.Token, request);

            var authQueryResponseParam = new AuthQueryResponse { AccessToken = accessToken, RefreshToken = refreshToken.Token };

            return authQueryResponseParam;
        }
        private async Task SaveRefreshTokensAsync(string refreshToken, RefreshTokenQuery request)
        {
            request.SharedExistentToken.Token = refreshToken;
            request.SharedExistentToken.ExpiryDateUtc = DateTime.UtcNow.AddDays(1);

            await _dbContext.SaveChangesAsync();

        }
    }
}
