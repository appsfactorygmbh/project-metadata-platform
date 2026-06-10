using System.Data.Common;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.IntegrationTests.Utilities;

public class PmpWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _ = builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(descriptor =>
                descriptor.ServiceType == typeof(DbContextOptions<ProjectMetadataPlatformDbContext>)
            );

            if (dbContextDescriptor != null)
            {
                _ = services.Remove(dbContextDescriptor);
            }

            var dbConnectionDescriptor = services.SingleOrDefault(descriptor =>
                descriptor.ServiceType == typeof(DbConnection)
            );

            if (dbConnectionDescriptor != null)
            {
                _ = services.Remove(dbConnectionDescriptor);
            }

            _ = services.AddDbContext<ProjectMetadataPlatformDbContext>(options =>
                options.UseSqlite("Datasource=unittest-db.db")
            );
        });

        _ = builder.UseEnvironment("Production");
    }
}
