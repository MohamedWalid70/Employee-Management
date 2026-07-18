using System.ComponentModel.DataAnnotations;

namespace Internship.EmployeeManagement.Api.Models.Users
{
    public class CreateUserCommandParam
    {
        [EmailAddress]
        [Required]
        public required string Email { get; set; }

        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,12}$", 
            ErrorMessage = "The password must contain at least one uppercase letter, one lowercase letter, one number and one special character and length must be between 8 and 12")]
        [Required]
        public required string Password { get; set; }
    }
}
