using System.Data.Common;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.IntegrationTests.Utilities;

public class PmpWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _ = builder.ConfigureServices(services =>
        {
            // 1. THE .NET 10 FIX: Remove the new internal configuration interface
            services.RemoveAll<IDbContextOptionsConfiguration<ProjectMetadataPlatformDbContext>>();

            // 2. Remove standard options and context
            services.RemoveAll<DbContextOptions<ProjectMetadataPlatformDbContext>>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<ProjectMetadataPlatformDbContext>();

            // 3. Remove connections and data sources (Crucial for modern Npgsql)
            services.RemoveAll<DbConnection>();
            services.RemoveAll<DbDataSource>();

            // 4. Inject SQLite context
            _ = services.AddDbContext<ProjectMetadataPlatformDbContext>(options =>
                options
                    .UseSqlite("Datasource=unittest-db.db")
                    .ConfigureWarnings(warnings =>
                        warnings.Ignore(
                            Microsoft
                                .EntityFrameworkCore
                                .Diagnostics
                                .RelationalEventId
                                .PendingModelChangesWarning
                        )
                    )
            );
        });

        _ = builder.UseEnvironment("Production");
    }
}
