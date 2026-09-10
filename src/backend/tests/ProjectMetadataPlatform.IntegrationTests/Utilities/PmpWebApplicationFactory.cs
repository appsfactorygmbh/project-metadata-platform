using System;
using System.Data.Common;
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
            services.RemoveAll<IDbContextOptionsConfiguration<ProjectMetadataPlatformDbContext>>();
            services.RemoveAll<DbContextOptions<ProjectMetadataPlatformDbContext>>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<ProjectMetadataPlatformDbContext>();
            services.RemoveAll<DbConnection>();
            services.RemoveAll<DbDataSource>();
            _ = services.AddDbContext<ProjectMetadataPlatformDbContext>(options =>
            {
                // Retrieve the connection details from the environment variables we set in OneTimeSetUp
                var host = Environment.GetEnvironmentVariable("PMP_DB_URL");
                var port = Environment.GetEnvironmentVariable("PMP_DB_PORT");
                var db = Environment.GetEnvironmentVariable("PMP_DB_NAME");
                var user = Environment.GetEnvironmentVariable("PMP_DB_USER");
                var pass = Environment.GetEnvironmentVariable("PMP_DB_PASSWORD");

                var connectionString =
                    $"Host={host};Port={port};Database={db};Username={user};Password={pass}";

                options
                    .UseNpgsql(connectionString)
                    .ConfigureWarnings(warnings =>
                        warnings.Ignore(
                            Microsoft
                                .EntityFrameworkCore
                                .Diagnostics
                                .RelationalEventId
                                .PendingModelChangesWarning
                        )
                    );
            });
        });

        _ = builder.UseEnvironment("Production");
    }
}
