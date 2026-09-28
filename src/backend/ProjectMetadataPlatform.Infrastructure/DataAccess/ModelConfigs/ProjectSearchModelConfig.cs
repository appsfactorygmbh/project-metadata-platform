using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess.ModelConfigs;

public class ProjectSearchModelConfig : IEntityTypeConfiguration<ProjectSearchModel>
{
    public void Configure(EntityTypeBuilder<ProjectSearchModel> builder)
    {
        _ = builder.HasKey(psm => psm.Id);
        _ = builder.ToView("mv_project_search");
    }
}
