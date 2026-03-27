using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

public class ApplicationUserModelConfig : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        //One to many relationship one team can have many users
        _ = builder.HasOne(u => u.Team).WithMany(t => t.Users).HasForeignKey(u => u.TeamId);

        // Many to many relationship many users can support many teams
        _ = builder.HasMany(u => u.TeamSupport).WithMany(t => t.TeamSupportUsers);
    }
}
