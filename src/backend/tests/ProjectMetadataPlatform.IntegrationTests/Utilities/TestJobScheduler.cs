using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.IntegrationTests.Utilities;

public class TestJobScheduler : IJobScheduler
{
    private readonly IServiceProvider _serviceProvider;

    public TestJobScheduler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ScheduleSearchViewRefreshAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext =
            scope.ServiceProvider.GetRequiredService<ProjectMetadataPlatformDbContext>();

        _ = await dbContext.Database.ExecuteSqlRawAsync(
            "REFRESH MATERIALIZED VIEW CONCURRENTLY mv_project_search;"
        );
    }
}
