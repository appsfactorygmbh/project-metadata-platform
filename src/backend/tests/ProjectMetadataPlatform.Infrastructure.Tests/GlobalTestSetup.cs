using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using Testcontainers.PostgreSql;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

[SetUpFixture]
public class GlobalTestSetup
{
    public static PostgreSqlContainer PostgresContainer { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        Environment.SetEnvironmentVariable("TESTCONTAINERS_RYUK_DISABLED", "true");

        PostgresContainer = new PostgreSqlBuilder("postgres:16-alpine").Build();

        await PostgresContainer.StartAsync();

        // Create database schema once for the entire test run
        var options = new DbContextOptionsBuilder<ProjectMetadataPlatformDbContext>()
            .UseNpgsql(PostgresContainer.GetConnectionString())
            .Options;

        using var context = new ProjectMetadataPlatformDbContext(options);
        await context.Database.MigrateAsync();
    }

    [OneTimeTearDown]
    public async Task RunAfterAllTests()
    {
        if (PostgresContainer != null)
        {
            await PostgresContainer.DisposeAsync();
        }
    }
}
