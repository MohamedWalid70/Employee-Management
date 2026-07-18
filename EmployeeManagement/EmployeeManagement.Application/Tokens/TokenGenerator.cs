using Internship.EmployeeManagement.Core.Entities;
using Internship.EmployeeManagement.Core.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Internship.EmployeeManagement.Application.Tokens
{
    public class TokenGenerator(IOptions<JwtSettings> options) : ITokenGenerator
    {
        private readonly JwtSettings _jwtSettings = options.Value;

        public string GenerateJwtToken(UserEntity user, IList<string> userRoles)
        {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(JwtRegisteredClaimNames.Email, user.Email ?? "")
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationPeriodInMinutes),
                SigningCredentials = credentials,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var tokenHandler = new JsonWebTokenHandler();

            string token = tokenHandler.CreateToken(tokenDescriptor);

            return token;
        }

        public RefreshTokenEntity GenerateRefreshToken(int userId)
        {

            var randomNumber = new byte[_jwtSettings.RefreshTokenSettings.Length];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);

            var refreshTokenEntity = new RefreshTokenEntity()
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiryDateUtc = DateTime.UtcNow.AddMinutes(_jwtSettings.RefreshTokenSettings.ExpirationPeriodInMinutes),
                UserId = userId
            };

            return refreshTokenEntity;
        }
    }
}
