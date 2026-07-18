using Internship.EmployeeManagement.Core.Entities;

namespace Internship.EmployeeManagement.Core.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddRefreshTokenAsync(RefreshTokenEntity refreshToken);
        void RemoveRefreshToken(RefreshTokenEntity refreshToken);
        Task<ICollection<RefreshTokenEntity>> GetRefreshTokensByUserIdAsync(int userId);
        Task<RefreshTokenEntity?> GetRefreshTokenByTokenAsync(string token);
        Task<int> RemoveRefreshTokensByUserIdAsync(int userId);
        Task<int> RemoveVulnerableRefreshTokensAsync(string exceptionToken, int userId);

    }
}
