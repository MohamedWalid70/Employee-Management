using Microsoft.AspNetCore.Identity;

namespace Internship.EmployeeManagement.Core.Entities
{
    public class UserEntity : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
