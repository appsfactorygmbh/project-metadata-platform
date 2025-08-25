using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

/// <summary>
/// Data Base Configuration for the Teams.
/// </summary>
public class TeamModelConfig : IEntityTypeConfiguration<Team>
{
    /// <summary>
    /// Configures the Team entity.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        // Set the primary key for the Team entity
        _ = builder.HasKey(e => e.Id);

        // One to many relation one Team can work on many projects
        _ = builder.HasMany(t => t.Projects).WithOne(p => p.Team).HasForeignKey(p => p.TeamId);

        // Team Name should be unique
        _ = builder.HasIndex(t => t.TeamName).IsUnique();
    }
}
