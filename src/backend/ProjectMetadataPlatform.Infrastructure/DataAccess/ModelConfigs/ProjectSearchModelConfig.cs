using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

/// <summary>
/// Data Base Configuration for the Project Search View.
/// </summary>
public class ProjectSearchModelConfig : IEntityTypeConfiguration<ProjectSearchModel>
{
    /// <summary>
    /// Configures the Project Search Model entity.
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<ProjectSearchModel> builder)
    {
        _ = builder.HasKey(psm => psm.Id);
        _ = builder.ToView("mv_project_search");
    }
}
