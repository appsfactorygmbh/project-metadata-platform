using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess;

/// <summary>
/// DbContext for the project metadata platform database.
/// </summary>
public sealed class ProjectMetadataPlatformDbContext : IdentityDbContext<IdentityUser>, IUnitOfWork
{
    /// <summary>
    /// Represents the table for the relation between Project and Plugin entities.
    /// </summary>
    public DbSet<ProjectPlugins> ProjectPluginsRelation { get; set; }

    /// <summary>
    /// Represents the table for plugin entities.
    /// </summary>
    public DbSet<Plugin> Plugins { get; set; }

    /// <summary>
    /// Represents the table for project entities.
    /// </summary>
    public DbSet<Project> Projects { get; set; }

    /// <summary>
    ///     Represents the table for log entities.
    /// </summary>
    public DbSet<Log> Logs { get; set; }

    /// <inheritdoc />
    public ProjectMetadataPlatformDbContext()
    {
    }

    /// <inheritdoc />
    public ProjectMetadataPlatformDbContext(DbContextOptions<ProjectMetadataPlatformDbContext> options) : base(options)
    {
    }

    /// <summary>
    ///     Configures the model that was discovered by convention from the entity types
    ///     exposed in DbSet properties on your derived context. The resulting model may be cached
    ///     and re-used for subsequent instances of your derived context.
    /// </summary>
    /// <param name="builder">
    ///     Provides a simple API surface for configuring a
    ///     Microsoft.EntityFrameworkCore.Metadata.IMutableModel that defines the shape of your entities, the relationships
    ///     between them, and how they map to the database.
    /// </param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        _ = builder.ApplyConfigurationsFromAssembly(typeof(ProjectMetadataPlatformDbContext).Assembly);

        SeedData(builder);
    }

    /// <summary>
    /// Seeds the database with initial data for projects and plugins.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure the database.</param>
    private static void SeedData(ModelBuilder modelBuilder)
    {
        var project1 = new Project
        {
            Id = 100,
            ProjectName = "DB App",
            Slug = "db_app",
            ClientName = "Deutsche Bahn",
            BusinessUnit = "Unit 1",
            TeamNumber = 1,
            Department = "Department 1"
        };

        var project2 = new Project
        {
            Id = 200,
            ProjectName = "Tagesschau App",
            Slug = "tagesschau_app",
            ClientName = "ARD",
            BusinessUnit = "Unit 2",
            TeamNumber = 2,
            Department = "Department 2"
        };

        var project3 = new Project
        {
            Id = 300,
            ProjectName = "AOK Bonus App",
            Slug = "aok_bonus_app",
            ClientName = "AOK",
            BusinessUnit = "Unit 3",
            TeamNumber = 3,
            Department = "Department 3"
        };

        var plugin1 = new Plugin { Id = 100, PluginName = "Gitlab" };

        var plugin2 = new Plugin { Id = 200, PluginName = "SonarQube" };

        var plugin3 = new Plugin { Id = 300, PluginName = "Jira" };

        _ = modelBuilder.Entity<Project>().HasData(project1, project2, project3);
        _ = modelBuilder.Entity<Plugin>().HasData(plugin1, plugin2, plugin3);

        _ = modelBuilder.Entity<ProjectPlugins>().HasData(
            new ProjectPlugins
            {
                ProjectId = project1.Id,
                PluginId = plugin1.Id,
                Url = "https://http.cat/status/100",
                DisplayName = "Gitlab",
                Project = null!,
                Plugin = null!
            },
            new ProjectPlugins
            {
                ProjectId = project1.Id,
                PluginId = plugin2.Id,
                Url = "https://http.cat/status/102",
                DisplayName = "SonarQube",
                Project = null!,
                Plugin = null!
            },
            new ProjectPlugins
            {
                ProjectId = project1.Id,
                PluginId = plugin3.Id,
                Url = "https://http.cat/status/200",
                DisplayName = "Jira",
                Project = null!,
                Plugin = null!
            },
            new ProjectPlugins
            {
                ProjectId = project2.Id,
                PluginId = plugin1.Id,
                Url = "https://http.cat/status/204",
                DisplayName = "Gitlab",
                Project = null!,
                Plugin = null!
            },
            new ProjectPlugins
            {
                ProjectId = project2.Id,
                PluginId = plugin2.Id,
                Url = "https://http.cat/status/401",
                DisplayName = "SonarQube",
                Project = null!,
                Plugin = null!
            },
            new ProjectPlugins
            {
                ProjectId = project2.Id,
                PluginId = plugin3.Id,
                Url = "https://http.cat/status/404",
                DisplayName = "Jira",
                Project = null!,
                Plugin = null!
            },
            new ProjectPlugins
            {
                ProjectId = project3.Id,
                PluginId = plugin1.Id,
                Url = "https://http.cat/status/406",
                DisplayName = "Gitlab",
                Project = null!,
                Plugin = null!
            },
            new ProjectPlugins
            {
                ProjectId = project3.Id,
                PluginId = plugin2.Id,
                Url = "https://http.cat/status/411",
                DisplayName = "SonarQube",
                Project = null!,
                Plugin = null!
            },
            new ProjectPlugins
            {
                ProjectId = project3.Id,
                PluginId = plugin3.Id,
                Url = "https://http.cat/status/414",
                DisplayName = "Jira",
                Project = null!,
                Plugin = null!
            });
    }

    /// <inheritdoc />
    public async Task CompleteAsync()
    {
        await SaveChangesAsync();
    }
}
