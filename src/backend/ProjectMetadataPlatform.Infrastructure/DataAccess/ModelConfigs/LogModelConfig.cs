using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

/// <summary>
/// Configuration of the Log Table in the database.
/// </summary>
public class LogModelConfig : IEntityTypeConfiguration<Log>
{
    /// <summary>
    /// Configures Log entity
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<Log> builder)
    {
        _ = builder.HasKey(e => e.Id);

        _ = builder
            .HasOne(e => e.Project)
            .WithMany(e => e.Logs)
            .HasForeignKey(e => e.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);

        _ = builder
            .HasOne(e => e.Author)
            .WithMany()
            .HasForeignKey(e => e.AuthorId)
            .OnDelete(DeleteBehavior.SetNull);

        _ = builder
            .HasOne(e => e.Team)
            .WithMany()
            .HasForeignKey(e => e.TeamId)
            .OnDelete(DeleteBehavior.SetNull);

        _ = builder
            .HasOne(e => e.AffectedUser)
            .WithMany()
            .HasForeignKey(e => e.AffectedUserId)
            .OnDelete(DeleteBehavior.SetNull);

        _ = builder
            .HasOne(e => e.GlobalPlugin)
            .WithMany()
            .HasForeignKey(e => e.GlobalPluginId)
            .OnDelete(DeleteBehavior.SetNull);

        _ = builder
            .HasOne(e => e.GlobalPlugin)
            .WithMany()
            .HasForeignKey(e => e.GlobalPluginId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
