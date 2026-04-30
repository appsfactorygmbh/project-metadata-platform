using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

/// <summary>
/// Data Base Configuration for the Api Tokens.
/// </summary>
public class ApiTokenModelConfig : IEntityTypeConfiguration<ApiToken>
{
    /// <summary>
    /// Configures the ApiToken entity.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<ApiToken> builder)
    {
        // Set the primary key for the ApiToken entity
        _ = builder.HasKey(t => t.Id);

        _ = builder.HasIndex(t => t.Name).IsUnique();
    }
}
