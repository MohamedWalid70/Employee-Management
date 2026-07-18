using Internship.EmployeeManagement.Core.Entities;
using Internship.EmployeeManagement.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Internship.EmployeeManagement.Infrastructure.Presistence.Repositories
{
    public class RefreshTokenRepository(IReadDbContext appDbContext) : IRefreshTokenRepository
    {
        private readonly IReadDbContext _appDbContext = appDbContext;

        public async Task AddRefreshTokenAsync(RefreshTokenEntity refreshToken)
        {
           await _appDbContext.RefreshTokens.AddAsync(refreshToken);
        }

        public async Task<RefreshTokenEntity?> GetRefreshTokenByTokenAsync(string token)
        {
            return await _appDbContext.RefreshTokens.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<ICollection<RefreshTokenEntity>> GetRefreshTokensByUserIdAsync(int userId)
        {
            return await _appDbContext.RefreshTokens.Where(rt => rt.UserId == userId).ToListAsync();
        }

        public void RemoveRefreshToken(RefreshTokenEntity refreshToken)
        {
            _appDbContext.RefreshTokens.Remove(refreshToken);
        }

        public async Task<int> RemoveRefreshTokensByUserIdAsync(int userId)
        {
            return await _appDbContext.RefreshTokens.Where(rt => rt.UserId == userId).ExecuteDeleteAsync();
        }

        public async Task<int> RemoveVulnerableRefreshTokensAsync(string exceptionToken, int userId)
        {
            return await _appDbContext.RefreshTokens.Where(rt => rt.UserId == userId && rt.Token != exceptionToken).ExecuteDeleteAsync();
        }
    }
}
