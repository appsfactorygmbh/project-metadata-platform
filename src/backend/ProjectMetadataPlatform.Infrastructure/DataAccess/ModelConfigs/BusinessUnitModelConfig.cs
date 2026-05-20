using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.BusinessUnits;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

public class BusinessUnitModelConfig : IEntityTypeConfiguration<BusinessUnit>
{
    /// <summary>
    /// Configures the BusinessUnit entity.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<BusinessUnit> builder)
    {

        _ = builder.HasKey(b => b.Id);

        _ = builder.HasIndex(b => b.BusinessUnitName).IsUnique();

        _ = builder
            .HasMany(b => b.Teams)
            .WithOne(t => t.BusinessUnit)
            .HasForeignKey(t => t.BusinessUnitId);

        _ = builder.HasMany(b => b.Users).WithMany(u => u.BusinessUnits);
    }
}
