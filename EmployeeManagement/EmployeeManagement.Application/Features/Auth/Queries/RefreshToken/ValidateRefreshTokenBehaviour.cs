using Internship.EmployeeManagement.Application.Exceptions;
using Internship.EmployeeManagement.Application.Features.Auth.Queries.Common;
using Internship.EmployeeManagement.Core.Entities;
using Internship.EmployeeManagement.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Internship.EmployeeManagement.Application.Features.Auth.Queries.RefreshToken
{
    public class ValidateRefreshTokenBehaviour(IHttpContextAccessor httpContextAccessor, IReadDbContext dbContext, IRefreshTokenRepository refreshTokenRepository) : IPipelineBehavior<RefreshTokenQuery, AuthQueryResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IReadDbContext _dbContext = dbContext;
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

        public async Task<AuthQueryResponse> Handle(RefreshTokenQuery request, RequestHandlerDelegate<AuthQueryResponse> next, CancellationToken cancellationToken)
        {
            var IsTokenValid = await CheckValidAccessTokenExistence();

            if (IsTokenValid)
            {
                var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

                var accesstoken = authorizationHeader?.StartsWith("Bearer ") == true ? authorizationHeader.Substring(7) : null;

                AuthQueryResponse authQueryResponseParam = new()
                {
                    AccessToken = accesstoken,
                    RefreshToken = request.RefreshToken
                };

                return authQueryResponseParam;
            }

            var tokenEntity = await ValidateRefreshToken(request.RefreshToken);

            request.SharedExistentToken = tokenEntity;

            var response = await next();

            return response;
        }

        private async Task<bool> CheckValidAccessTokenExistence()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            return userIdClaim is not null;
        }

        private async Task<RefreshTokenEntity> ValidateRefreshToken(string token)
        {
            var refreshTokenEntity = await _refreshTokenRepository.GetRefreshTokenByTokenAsync(token);

            if (refreshTokenEntity is null)
                throw new BadRequestException("Invalid signature! Please, Login again");

            await CheckTokenExpiration(refreshTokenEntity);

            var associatedRefreshTokens = await _refreshTokenRepository.GetRefreshTokensByUserIdAsync(refreshTokenEntity.UserId);

            var latestToken = associatedRefreshTokens.OrderByDescending(rt => rt.ExpiryDateUtc).FirstOrDefault();

            await CheckAndRevokePossibleOldTokens(refreshTokenEntity, associatedRefreshTokens, latestToken!);

            return latestToken!;
        }

        private async Task CheckAndRevokePossibleOldTokens(RefreshTokenEntity refreshTokenEntity, ICollection<RefreshTokenEntity> associatedRefreshTokens, RefreshTokenEntity latestRefreshToken)
        {
            if (latestRefreshToken?.Id != refreshTokenEntity.Id)
            {
                await _refreshTokenRepository.RemoveRefreshTokensByUserIdAsync(refreshTokenEntity.UserId);

                throw new BadRequestException("Invalid signature! Please, Login again");
            }
            else if (latestRefreshToken?.Id == refreshTokenEntity.Id && associatedRefreshTokens.Count > 1)
            {
                await _refreshTokenRepository.RemoveVulnerableRefreshTokensAsync(refreshTokenEntity.Token, refreshTokenEntity.UserId);
            }
        }

        private async Task CheckTokenExpiration(RefreshTokenEntity refreshTokenEntity)
        {
            if (refreshTokenEntity?.ExpiryDateUtc < DateTime.UtcNow)
            {
                _refreshTokenRepository.RemoveRefreshToken(refreshTokenEntity);

                await _dbContext.SaveChangesAsync();

                throw new BadRequestException("Invalid expired signature! Please, Login again");
            }
        }

    }
}
