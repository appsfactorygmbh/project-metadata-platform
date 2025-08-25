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
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(descriptor =>
                descriptor.ServiceType == typeof(DbContextOptions<ProjectMetadataPlatformDbContext>)
            );

            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            var dbConnectionDescriptor = services.SingleOrDefault(descriptor =>
                descriptor.ServiceType == typeof(DbConnection)
            );

            if (dbConnectionDescriptor != null)
            {
                services.Remove(dbConnectionDescriptor);
            }

            services.AddDbContext<ProjectMetadataPlatformDbContext>(options =>
                options.UseSqlite("Datasource=unittest-db.db")
            );
        });

        builder.UseEnvironment("Production");
    }
}
