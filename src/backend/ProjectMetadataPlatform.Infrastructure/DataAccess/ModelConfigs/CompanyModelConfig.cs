using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.Companies;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

/// <summary>
/// Data Base Configuration for the Companies.
/// </summary>
public class CompanyModelConfig : IEntityTypeConfiguration<Company>
{
    /// <summary>
    /// Configures the Company entity.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        _ = builder.HasKey(c => c.Id);

        _ = builder.HasIndex(c => c.CompanyName).IsUnique();

        _ = builder
            .HasMany(c => c.Projects)
            .WithOne(p => p.Company)
            .HasForeignKey(p => p.CompanyId);

        _ = builder.HasMany(c => c.Users).WithOne(u => u.Company).HasForeignKey(u => u.CompanyId);
    }
}
