using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Infrastructure.Plugins;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

public class PluginsRepositoryTest : TestsWithDatabase
{
    private ProjectMetadataPlatformDbContext _context;
    private PluginRepository _repository;

    [SetUp]
    public void Setup()
    {
        _context = DbContext();
        _repository = new PluginRepository(_context);
        ClearData(_context);
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up the database after each test
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task TestPluginRepository()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Regen",
            Slug = "regen",
            ClientName = "Nasa",
        };

        _context.Projects.Add(project);

        var plugin = new Plugin { Id = 1, PluginName = "Gitlab" };
        _context.Plugins.Add(plugin);

        var projectPluginRelation = new ProjectPlugins
        {
            PluginId = 1,
            ProjectId = 1,
            Plugin = plugin,
            Project = project,
            Url = "gitlab.com",
            DisplayName = "gitlab",
        };
        _context.Add(projectPluginRelation);
        await _context.SaveChangesAsync();

        var rep = await _repository.GetAllPluginsForProjectIdAsync(1);

        Assert.That(rep, Is.Not.Empty);

        Assert.Multiple(() =>
        {
            Assert.That(rep[0].Url, Is.EqualTo("gitlab.com"));
            Assert.That(rep[0].DisplayName, Is.EqualTo("gitlab"));
            Assert.That(rep[0].Plugin?.PluginName, Is.EqualTo("Gitlab"));
        });
    }

    [Test]
    public async Task CreatePlugin_Test()
    {
        var examplePlugin = new Plugin { PluginName = "Warp-Drive", ProjectPlugins = [] };

        var plugin = await _repository.StorePlugin(examplePlugin);

        Assert.That(plugin, Is.Not.Null);
        Assert.That(plugin.PluginName, Is.EqualTo("Warp-Drive"));
    }

    [Test]
    public async Task CreatePlugins_IdsDifferent_Test()
    {
        var pluginMethane = new Plugin { PluginName = "Methane", ProjectPlugins = [] };
        var pluginOxygen = new Plugin { PluginName = "Oxygen", ProjectPlugins = [] };

        var pluginOne = await _repository.StorePlugin(pluginMethane);
        var pluginTwo = await _repository.StorePlugin(pluginOxygen);
        await _context.SaveChangesAsync();

        Assert.Multiple(() =>
        {
            Assert.That(pluginOne, Is.Not.Null);
            Assert.That(pluginTwo, Is.Not.Null);
        });

        Assert.That(pluginOne.Id, Is.Not.EqualTo(pluginTwo.Id));
    }

    [Test]
    public async Task StorePlugin_NoIdIncrementWhenIdExists_Test()
    {
        var examplePlugin = new Plugin
        {
            PluginName = "Warp-Drive",
            ProjectPlugins = [],
            Id = 42,
        };
        _context.Add(examplePlugin);
        await _context.SaveChangesAsync();

        examplePlugin.PluginName = "Hall Effect Thruster";

        var plugin = await _repository.StorePlugin(examplePlugin);

        Assert.That(plugin, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(plugin.PluginName, Is.EqualTo("Hall Effect Thruster"));
            Assert.That(plugin.Id, Is.EqualTo(42));
        });
    }

    [Test]
    public async Task GetGlobalPluginById_Test()
    {
        var examplePlugin = new Plugin
        {
            PluginName = "Warp-Drive",
            ProjectPlugins = [],
            Id = 42,
        };
        _context.Add(examplePlugin);
        await _context.SaveChangesAsync();

        var plugin = await _repository.GetPluginByIdAsync(42);

        Assert.That(plugin, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(plugin.PluginName, Is.EqualTo("Warp-Drive"));
            Assert.That(plugin.Id, Is.EqualTo(42));
        });
    }

    [Test]
    public void GetGlobalPluginById_NotFound_Test()
    {
        Assert.ThrowsAsync<PluginNotFoundException>(() => _repository.GetPluginByIdAsync(42));
    }

    [Test]
    public async Task GetGlobalPlugins_Test()
    {
        var examplePlugin = new Plugin
        {
            PluginName = "Warp-Drive",
            ProjectPlugins = [],
            Id = 42,
        };
        _context.Add(examplePlugin);
        await _context.SaveChangesAsync();

        var plugin = (await _repository.GetGlobalPluginsAsync()).ToList();

        Assert.That(plugin, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(plugin.First().PluginName, Is.EqualTo("Warp-Drive"));
            Assert.That(plugin.First().Id, Is.EqualTo(42));
        });
    }

    [Test]
    public async Task GetGlobalPlugins_NoPlugins_Test()
    {
        var plugin = await _repository.GetGlobalPluginsAsync();

        Assert.That(plugin, Is.Empty);
    }

    [Test]
    public async Task GetAllUnarchivedPluginsForProjectIdAsync_ShouldReturnOnlyUnarchivedPlugins()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Test Project",
            Slug = "test_project",
            ClientName = "Test Client", // Ensure ClientName is set
        };

        var unarchivedPlugin = new Plugin
        {
            Id = 1,
            PluginName = "Unarchived Plugin",
            IsArchived = false,
        };
        var archivedPlugin = new Plugin
        {
            Id = 2,
            PluginName = "Archived Plugin",
            IsArchived = true,
        };
        var projectPluginRelation1 = new ProjectPlugins
        {
            ProjectId = 1,
            PluginId = 1,
            Plugin = unarchivedPlugin,
            Project = project,
            Url = "unarchived.com",
        };
        var projectPluginRelation2 = new ProjectPlugins
        {
            ProjectId = 1,
            PluginId = 2,
            Plugin = archivedPlugin,
            Project = project,
            Url = "archived.com",
        };
        _context.Projects.Add(project);
        _context.Plugins.AddRange(unarchivedPlugin, archivedPlugin);
        _context.ProjectPluginsRelation.AddRange(projectPluginRelation1, projectPluginRelation2);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllUnarchivedPluginsForProjectIdAsync(1);

        Assert.That(result, Has.Count.EqualTo(1)); // Only unarchived plugins should be returned
        Assert.That(result[0].Plugin?.PluginName, Is.EqualTo("Unarchived Plugin"));
    }

    [Test]
    public async Task GetAllUnarchivedPluginsForProjectIdAsync_ShouldReturnEmptyWhenNoUnarchivedPlugins()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Test Project",
            Slug = "test_project",
            ClientName = "Test Client", // Make sure this is set
        };
        var archivedPlugin = new Plugin
        {
            Id = 1,
            PluginName = "Archived Plugin",
            IsArchived = true,
        };
        var projectPluginRelation = new ProjectPlugins
        {
            ProjectId = 1,
            PluginId = 1,
            Plugin = archivedPlugin,
            Project = project,
            Url = "archived.com",
        };
        _context.Projects.Add(project);
        _context.Plugins.Add(archivedPlugin);
        _context.ProjectPluginsRelation.Add(projectPluginRelation);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllUnarchivedPluginsForProjectIdAsync(1);

        Assert.That(result, Is.Empty); // No unarchived plugins should be returned
    }

    [Test]
    public async Task GetAllUnarchivedPluginsForProjectIdAsync_ShouldReturnEmptyWhenNoPluginsForProject()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Test Project",
            Slug = "test_project",
            ClientName = "Test Client", // Make sure this is set
        };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllUnarchivedPluginsForProjectIdAsync(1);

        Assert.That(result, Is.Empty); // No plugins should be associated with the project
    }

    [Test]
    public async Task GetAllUnarchivedPluginsForProjectIdAsync_ShouldReturnEmptyWhenAllPluginsAreArchived()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Test Project",
            Slug = "test_project",
            ClientName = "Test Client", // Make sure this is set
        };
        var archivedPlugin = new Plugin
        {
            Id = 1,
            PluginName = "Archived Plugin",
            IsArchived = true,
        };
        var projectPluginRelation = new ProjectPlugins
        {
            ProjectId = 1,
            PluginId = 1,
            Plugin = archivedPlugin,
            Project = project,
            Url = "archived.com",
        };
        _context.Projects.Add(project);
        _context.Plugins.Add(archivedPlugin);
        _context.ProjectPluginsRelation.Add(projectPluginRelation);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllUnarchivedPluginsForProjectIdAsync(1);

        // Assert
        Assert.That(result, Is.Empty); // All plugins are archived, so no results
    }

    [Test]
    public async Task GetAllUnarchivedPluginsForProjectIdAsync_ShouldReturnOnlyUnarchivedWhenMixOfArchivedAndUnarchived()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Test Project",
            Slug = "test_project",
            ClientName = "Test Client", // Make sure this is set
        };
        var unarchivedPlugin = new Plugin
        {
            Id = 1,
            PluginName = "Unarchived Plugin",
            IsArchived = false,
        };
        var archivedPlugin = new Plugin
        {
            Id = 2,
            PluginName = "Archived Plugin",
            IsArchived = true,
        };
        var projectPluginRelation1 = new ProjectPlugins
        {
            ProjectId = 1,
            PluginId = 1,
            Plugin = unarchivedPlugin,
            Project = project,
            Url = "unarchived.com",
        };
        var projectPluginRelation2 = new ProjectPlugins
        {
            ProjectId = 1,
            PluginId = 2,
            Plugin = archivedPlugin,
            Project = project,
            Url = "archived.com",
        };
        _context.Projects.Add(project);
        _context.Plugins.AddRange(unarchivedPlugin, archivedPlugin);
        _context.ProjectPluginsRelation.AddRange(projectPluginRelation1, projectPluginRelation2);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllUnarchivedPluginsForProjectIdAsync(1);

        Assert.That(result, Has.Count.EqualTo(1)); // Only unarchived plugins should be returned
        Assert.That(result[0].Plugin?.PluginName, Is.EqualTo("Unarchived Plugin"));
    }

    [Test]
    public async Task GetAllUnarchivedPluginsForProjectIdAsync_ShouldReturnPluginsBelongingToTheSpecifiedProject()
    {
        var project1 = new Project
        {
            Id = 1,
            ProjectName = "Test Project",
            Slug = "test_project",
            ClientName = "Test Client", // Make sure this is set
        };
        var project2 = new Project
        {
            Id = 2,
            ProjectName = "Test Project2",
            Slug = "test_project2",
            ClientName = "Test Client2", // Make sure this is set
        };
        var unarchivedPlugin = new Plugin
        {
            Id = 1,
            PluginName = "Unarchived Plugin",
            IsArchived = false,
        };

        var projectPluginRelation1 = new ProjectPlugins
        {
            ProjectId = 1,
            PluginId = 1,
            Plugin = unarchivedPlugin,
            Project = project1,
            Url = "plugin1.com",
        };
        var projectPluginRelation2 = new ProjectPlugins
        {
            ProjectId = 2,
            PluginId = 1,
            Plugin = unarchivedPlugin,
            Project = project2,
            Url = "plugin2.com",
        };

        _context.Projects.AddRange(project1, project2);
        _context.Plugins.Add(unarchivedPlugin);
        _context.ProjectPluginsRelation.AddRange(projectPluginRelation1, projectPluginRelation2);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllUnarchivedPluginsForProjectIdAsync(1);

        Assert.That(result, Has.Count.EqualTo(1)); // Only the plugin for project 1 should be returned
        Assert.That(result[0].Plugin?.PluginName, Is.EqualTo("Unarchived Plugin"));
    }

    [Test]
    public void TestGetPluginsForNonExistentProjectThrowsException()
    {
        const int nonExistentProjectId = 999;

        var ex = Assert.ThrowsAsync<ProjectNotFoundException>(async () =>
        {
            await _repository.GetAllUnarchivedPluginsForProjectIdAsync(nonExistentProjectId);
        });

        Assert.That(ex.Message, Is.EqualTo("The project with id 999 was not found."));
    }

    [Test]
    public async Task TestDeletePlugins()
    {
        // Arrange
        var project1 = new Project
        {
            Id = 1,
            ProjectName = "Test Project",
            ClientName = "Test Client",
            Slug = "testProject",
        };
        var project2 = new Project
        {
            Id = 2,
            ProjectName = "Test Project2",
            ClientName = "Test Client2",
            Slug = "testProject2",
        };
        var archivedPlugin = new Plugin
        {
            Id = 1,
            PluginName = "Unarchived Plugin",
            IsArchived = true,
        };

        var projectPluginRelation1 = new ProjectPlugins
        {
            ProjectId = 1,
            PluginId = 1,
            Plugin = archivedPlugin,
            Project = project1,
            Url = "plugin1.com",
        };
        var projectPluginRelation2 = new ProjectPlugins
        {
            ProjectId = 2,
            PluginId = 1,
            Plugin = archivedPlugin,
            Project = project2,
            Url = "plugin2.com",
        };

        _context.Projects.AddRange(project1, project2);
        _context.Plugins.Add(archivedPlugin);
        _context.ProjectPluginsRelation.AddRange(projectPluginRelation1, projectPluginRelation2);

        await _context.SaveChangesAsync();

        // Act
        var returnValDeleteGlobalPlugin = await _repository.DeleteGlobalPlugin(archivedPlugin);

        // Assert
        Assert.That(returnValDeleteGlobalPlugin, Is.True);

        _context.Entry(project1).State = EntityState.Detached;
        _context.Entry(project2).State = EntityState.Detached;

        var reloadedProject1 = await _context
            .Projects.Include(p => p.ProjectPlugins)
            .FirstOrDefaultAsync(p => p.Id == 1);
        var reloadedProject2 = await _context
            .Projects.Include(p => p.ProjectPlugins)
            .FirstOrDefaultAsync(p => p.Id == 2);
        Assert.Multiple(() =>
        {
            Assert.That(reloadedProject1, Is.Not.Null);
            Assert.That(reloadedProject2, Is.Not.Null);
            Assert.That(reloadedProject1?.ProjectPlugins, Is.Empty);
            Assert.That(reloadedProject2?.ProjectPlugins, Is.Empty);
        });
    }

    [Test]
    public async Task CheckPluginNameExists_Test()
    {
        var plugin = new Plugin { Id = 1, PluginName = "Gitlab" };
        _context.Plugins.Add(plugin);

        await _context.SaveChangesAsync();

        var result = await _repository.CheckGlobalPluginNameExists("Gitlab");

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckPluginNameExists_Not_Test()
    {
        var result = await _repository.CheckGlobalPluginNameExists("Bielefeld");

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CheckPluginNameExistsChecksCaseInsentive_Test()
    {
        var plugin = new Plugin { Id = 1, PluginName = "Gitlab" };
        _context.Plugins.Add(plugin);

        await _context.SaveChangesAsync();

        var result = await _repository.CheckGlobalPluginNameExists("gitLaB");

        Assert.That(result, Is.True);
    }
}
