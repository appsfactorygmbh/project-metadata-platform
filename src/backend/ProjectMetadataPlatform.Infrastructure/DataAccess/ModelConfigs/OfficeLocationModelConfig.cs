using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

/// <summary>
/// Data Base Configuration for the Office Locations.
/// </summary>
public class OfficeLocationModelConfig : IEntityTypeConfiguration<OfficeLocation>
{
    /// <summary>
    /// Configures the Office Location entity.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<OfficeLocation> builder)
    {
        _ = builder.HasKey(o => o.Id);

        _ = builder.HasIndex(o => o.OfficeLocationName).IsUnique();

        _ = builder
            .HasMany(o => o.Users)
            .WithOne(u => u.OfficeLocation)
            .HasForeignKey(u => u.OfficeLocationId);
    }
}
