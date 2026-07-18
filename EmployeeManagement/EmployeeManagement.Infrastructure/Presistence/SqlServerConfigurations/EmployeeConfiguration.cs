using Internship.EmployeeManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Internship.EmployeeManagement.Infrastructure.Presistence.SqlServerConfigurations
{
    public class EmployeeConfiguration: IEntityTypeConfiguration<EmployeeEntity>
    {
        public void Configure(EntityTypeBuilder<EmployeeEntity> builder)
        {

            builder.ToTable("Employees", table => table.IsTemporal().UseHistoryTable("EmployeesHistory"));
        }
    }
}
