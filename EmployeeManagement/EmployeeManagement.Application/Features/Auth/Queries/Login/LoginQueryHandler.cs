using Internship.EmployeeManagement.Application.Features.Auth.Queries.Common;
using Internship.EmployeeManagement.Core.Entities;
using Internship.EmployeeManagement.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Internship.EmployeeManagement.Application.Features.Auth.Queries.Login
{
    public class LoginQueryHandler(UserManager<UserEntity> userManager, ITokenGenerator tokenGenerator, IRefreshTokenRepository refreshTokenRepository, IReadDbContext dbContext) : IRequestHandler<LoginQuery, AuthQueryResponse>
    {
        private readonly UserManager<UserEntity> _userManager = userManager;
        private readonly ITokenGenerator _tokenGenerator = tokenGenerator;
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
        private readonly IReadDbContext _dbContext = dbContext;

        public async Task<AuthQueryResponse> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var userRoles = await _userManager.GetRolesAsync(request.SharedUser);

            var accessToken = _tokenGenerator.GenerateJwtToken(request.SharedUser, userRoles);

            var refreshToken = _tokenGenerator.GenerateRefreshToken(request.SharedUser.Id);

            await SaveRefreshTokensAsync(refreshToken);

            var authQueryResponseParam = new AuthQueryResponse { AccessToken = accessToken, RefreshToken = refreshToken.Token };

            return authQueryResponseParam;
        }

        private async Task SaveRefreshTokensAsync(RefreshTokenEntity refreshToken)
        {
            await _refreshTokenRepository.AddRefreshTokenAsync(refreshToken);

            await _dbContext.SaveChangesAsync();

        }

    }
}
