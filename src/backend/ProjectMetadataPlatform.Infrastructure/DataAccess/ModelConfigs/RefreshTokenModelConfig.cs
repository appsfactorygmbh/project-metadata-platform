using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

/// <summary>
/// Data Base Configuration for the Refresh Tokens.
/// </summary>
public class RefreshTokenModelConfig : IEntityTypeConfiguration<RefreshToken>
{
    /// <summary>
    /// Configures the RefreshToken entity.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        // Set the primary key for the RefreshToken entity
        _ = builder.HasKey(e => e.Id);

        _ = builder.HasOne(pp => pp.User).WithMany().HasForeignKey(pp => pp.UserId);
    }
}
