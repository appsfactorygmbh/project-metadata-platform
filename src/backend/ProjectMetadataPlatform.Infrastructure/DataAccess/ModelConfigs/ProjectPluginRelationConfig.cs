using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

/// <summary>
/// Data Base Configuration for the relation between Project and Plugin.
/// </summary>
public class ProjectPluginRelationConfig : IEntityTypeConfiguration<ProjectPlugins>
{
    /// <summary>
    /// Configures the ProjectPlugins entity.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<ProjectPlugins> builder)
    {
        _ = builder.HasKey(pp => new
        {
            pp.PluginId,
            pp.ProjectId,
            pp.Url,
        });

        _ = builder
            .HasOne(pp => pp.Project)
            .WithMany(p => p.ProjectPlugins)
            .HasForeignKey(pp => pp.ProjectId);

        _ = builder
            .HasOne(pp => pp.Plugin)
            .WithMany(p => p.ProjectPlugins)
            .HasForeignKey(pp => pp.PluginId);
    }
}
