using Internship.EmployeeManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Internship.EmployeeManagement.Infrastructure.Presistence.SqlServerConfigurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshTokenEntity>
    {
        public void Configure(EntityTypeBuilder<RefreshTokenEntity> builder)
        {
            builder.ToTable("RefreshTokens");
            builder.HasOne(rt => rt.User).WithMany().HasForeignKey(rt => rt.UserId);
            builder.HasIndex(rt => rt.Token).IsUnique();
            builder.Property(rt => rt.Token).HasMaxLength(150);
        }
    }
}
