namespace Internship.EmployeeManagement.Core.Entities
{
    public class RefreshTokenEntity
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public int UserId { get; set; }
        public UserEntity User { get; set; }
        public DateTime ExpiryDateUtc { get; set; }
    }
}
