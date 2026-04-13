using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

/// <summary>
/// Data Base Configuration for the Application Users.
/// </summary>
public class ApplicationUserModelConfig : IEntityTypeConfiguration<ApplicationUser>
{
    /// <summary>
    /// Configures ApplicationUser entity
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        //One to many relationship one team can have many users
        _ = builder.HasMany(u => u.Teams).WithMany(t => t.Users);

        // Many to many relationship many users can support many teams
        _ = builder.HasMany(u => u.TeamSupport).WithMany(t => t.TeamSupportUsers);

        _ = builder.HasIndex(u => u.EmployeeId).IsUnique();
        _ = builder.Property(e => e.EmployeeId).IsRequired();
    }
}
