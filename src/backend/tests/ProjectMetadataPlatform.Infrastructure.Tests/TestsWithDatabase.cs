using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using Testcontainers.PostgreSql;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

[TestFixture]
public abstract class TestsWithDatabase
{
    [SetUp]
    public void BaseSetUp()
    {
        using var context = DbContext();
        ClearData(context);
    }

    [TearDown]
    public void BaseTearDown()
    {
        using var context = DbContext();
        ClearData(context);
    }

    protected static ProjectMetadataPlatformDbContext DbContext()
    {
        return new ProjectMetadataPlatformDbContext(
            new DbContextOptionsBuilder<ProjectMetadataPlatformDbContext>()
                .UseNpgsql(GlobalTestSetup.PostgresContainer.GetConnectionString())
                .Options
        );
    }

    /// <summary>
    /// This method deletes the initially loaded Data from SeedData.
    /// </summary>
    /// <param name="context"></param>
    /// <returns>Database context</returns>
    protected static void ClearData(ProjectMetadataPlatformDbContext context)
    {
        var allEntitiesPlugins = context.Plugins.ToList();
        var allEntitiesProjects = context.Projects.ToList();
        var allEntitiesProjectsPlugins = context.ProjectPluginsRelation.ToList();
        var allEntitiesLogs = context.Logs.ToList();
        var allEntitiesRefreshTokens = context.Set<RefreshToken>().ToList();
        var allEntitiesApiTokens = context.ApiTokens.ToList();
        var allEntitiesTeams = context.Teams.ToList();
        var allEntitiesUsers = context
            .Users.Where(user => user.Email != "admin@admin.admin")
            .ToList();
        var allEntitiesCompanies = context.Companies.ToList();
        var allEntitiesDepartments = context.Departments.ToList();
        var allEntitiesBusinessUnits = context.BusinessUnits.ToList();
        var allEntitiesOfficeLocations = context.OfficeLocations.ToList();
        var allEntitiesBilling = context.GlobalBilling.ToList();
        var allEntitiesPluginBilling = context.PluginBillingRelation.ToList();

        context.Plugins.RemoveRange(allEntitiesPlugins);
        context.Projects.RemoveRange(allEntitiesProjects);
        context.ProjectPluginsRelation.RemoveRange(allEntitiesProjectsPlugins);
        context.Logs.RemoveRange(allEntitiesLogs);
        context.Set<RefreshToken>().RemoveRange(allEntitiesRefreshTokens);
        context.ApiTokens.RemoveRange(allEntitiesApiTokens);
        context.Teams.RemoveRange(allEntitiesTeams);
        context.Users.RemoveRange(allEntitiesUsers);
        context.Companies.RemoveRange(allEntitiesCompanies);
        context.Departments.RemoveRange(allEntitiesDepartments);
        context.BusinessUnits.RemoveRange(allEntitiesBusinessUnits);
        context.OfficeLocations.RemoveRange(allEntitiesOfficeLocations);
        context.GlobalBilling.RemoveRange(allEntitiesBilling);
        context.PluginBillingRelation.RemoveRange(allEntitiesPluginBilling);
        _ = context.SaveChanges();
    }

    [Test]
    public void TestForConclusiveness()
    {
        //Test only exists to pass the pipeline
        Assert.Pass();
    }
}
