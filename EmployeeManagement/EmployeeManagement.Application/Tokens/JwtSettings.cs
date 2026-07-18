namespace Internship.EmployeeManagement.Application.Tokens
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpirationPeriodInMinutes { get; set; }
        public string Token { get; set; }
        public RefreshTokenSettings RefreshTokenSettings { get; set; }
    }
}
