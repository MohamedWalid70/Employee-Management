using Internship.EmployeeManagement.Core.Entities;

namespace Internship.EmployeeManagement.Core.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateJwtToken(UserEntity user, IList<string> userRoles);
        RefreshTokenEntity GenerateRefreshToken(int userId);
    }
}
