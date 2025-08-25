using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

/// <summary>
/// Data Base Configuration for the Projects.
/// </summary>
public class ProjectModelConfig : IEntityTypeConfiguration<Project>
{
    /// <summary>
    /// Configures the Project entity.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        // Set the primary key for the Project entity
        _ = builder.HasKey(e => e.Id);

        _ = builder
            .HasMany(p => p.ProjectPlugins)
            .WithOne(pp => pp.Project)
            .HasForeignKey(pp => pp.ProjectId);

        // relationship a project has one team
        _ = builder
            .HasOne(p => p.Team)
            .WithMany(t => t.Projects)
            .HasForeignKey(p => p.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = builder.HasIndex(p => p.Slug).IsUnique();
    }
}
