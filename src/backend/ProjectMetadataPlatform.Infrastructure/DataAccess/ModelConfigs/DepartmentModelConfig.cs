using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Companies;
using ProjectMetadataPlatform.Domain.Departments;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

public class DepartmentModelConfig : IEntityTypeConfiguration<Department>
{
    /// <summary>
    /// Configures the Department entity.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        _ = builder.HasKey(d => d.Id);

        _ = builder.HasIndex(d => d.DepartmentName).IsUnique();

        _ = builder.HasMany(d => d.Users).WithMany(u => u.Departments);
    }
}
