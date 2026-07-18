using System.ComponentModel.DataAnnotations;

namespace Internship.EmployeeManagement.Api.Models.Employees
{
    public class UpdateEmployeeCommandParam
    { 
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string Name { get; set; }
        
        [Range(18, 50)]
        public byte Age { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string Title { get; set; }
    }

}
