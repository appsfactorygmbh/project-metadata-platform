using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.Billing;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

/// <summary>
/// Data Base Configuration for the GlobalBilling.
/// </summary>
public class GlobalBillingModelConfig : IEntityTypeConfiguration<GlobalBilling>
{
    /// <summary>
    /// Configures the GlobalBilling entity.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<GlobalBilling> builder)
    {
        _ = builder.HasKey(d => d.Id);

        _ = builder.HasIndex(d => d.BillingKind).IsUnique();

        _ = builder
            .HasMany(g => g.PluginBilling)
            .WithOne(p => p.GlobalBilling)
            .HasForeignKey(p => p.BillingId);
    }
}
