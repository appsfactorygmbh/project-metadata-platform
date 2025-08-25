using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

/// <summary>
/// Data Base Configuration for the Plugins.
/// </summary>
public class PluginModelConfig : IEntityTypeConfiguration<Plugin>
{
    /// <summary>
    /// Configures the Plugin entity.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<Plugin> builder)
    {
        _ = builder.HasKey(e => e.Id);

        _ = builder
            .HasMany(p => p.ProjectPlugins)
            .WithOne(pp => pp.Plugin)
            .HasForeignKey(pp => pp.PluginId);

        builder.HasIndex(plugin => plugin.PluginName).IsUnique();
    }
}
