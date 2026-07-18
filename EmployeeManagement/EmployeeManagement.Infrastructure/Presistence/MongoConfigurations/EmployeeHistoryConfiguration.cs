using Internship.EmployeeManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Internship.EmployeeManagement.Infrastructure.Presistence.MongoConfigurations
{
    public class EmployeeHistoryConfiguration : IEntityTypeConfiguration<EmployeeHistoryEntity>
    {
        public void Configure(EntityTypeBuilder<EmployeeHistoryEntity> builder)
        {
            builder.ToTable("EmployeeHistory");
            builder.HasKey(eh => eh.Id);
        }
    }
}
