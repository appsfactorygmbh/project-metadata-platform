using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.Billing;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

/// <summary>
/// Data Base Configuration for the relation between plugins and billing.
/// </summary>
public class PluginBillingRelationModelConfig : IEntityTypeConfiguration<PluginBilling>
{
    /// <summary>
    /// Configures the PluginBilling entity.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<PluginBilling> builder)
    {
        _ = builder.HasKey(pb => new { pb.ProjectId, pb.PluginId });

        _ = builder
            .HasOne(pb => pb.GlobalBilling)
            .WithMany(b => b.PluginBilling)
            .HasForeignKey(pb => pb.BillingId);

        _ = builder
            .HasOne(pb => pb.ProjectPlugin)
            .WithOne(pp => pp.PluginBilling)
            .HasForeignKey<PluginBilling>(pb => new { pb.ProjectId, pb.PluginId })
            .HasPrincipalKey<ProjectPlugin>(pp => new { pp.ProjectId, pp.Id });
    }
}
