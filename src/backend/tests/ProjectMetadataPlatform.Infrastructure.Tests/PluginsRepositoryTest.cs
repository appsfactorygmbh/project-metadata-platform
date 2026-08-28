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
        _ = _context.Database.EnsureDeleted();
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
            CompanyId = 1,
        };

        _ = _context.Projects.Add(project);

        var plugin = new Plugin { Id = 1, PluginName = "Gitlab" };
        _ = _context.Plugins.Add(plugin);

        var projectPluginRelation = new ProjectPlugin
        {
            PluginId = 1,
            ProjectId = 1,
            Plugin = plugin,
            Project = project,
            Url = "gitlab.com",
            DisplayName = "gitlab",
        };
        _ = _context.Add(projectPluginRelation);
        _ = await _context.SaveChangesAsync();

        var rep = await (await _repository.GetAllPluginsForProjectIdAsync(1)).ToListAsync();

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
        _ = await _context.SaveChangesAsync();

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
        _ = _context.Add(examplePlugin);
        _ = await _context.SaveChangesAsync();

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
    public async Task CreateProjectPlugin_Test()
    {
        var exampleProjectPlugin = new ProjectPlugin
        {
            DisplayName = "Warp-Drive",
            Url = "123",
            PluginId = 1,
        };

        var plugin = await _repository.StoreProjectPlugin(exampleProjectPlugin);

        Assert.That(plugin, Is.Not.Null);
        Assert.That(plugin.DisplayName, Is.EqualTo("Warp-Drive"));
    }

    [Test]
    public async Task CreateProjectPlugins_IdsDifferent_Test()
    {
        _context.Projects.Add(
            new Project
            {
                Id = 1,
                ProjectName = "A",
                Slug = "a",
                ClientName = "1",
                CompanyId = 1,
            }
        );
        _context.Plugins.Add(
            new Plugin
            {
                PluginName = "Warp-Drive",
                ProjectPlugins = [],
                Id = 1,
            }
        );
        await _context.SaveChangesAsync();
        var pluginMethane = new ProjectPlugin
        {
            DisplayName = "Methane",
            Url = "1235",
            PluginId = 1,
            ProjectId = 1,
        };
        var pluginOxygen = new ProjectPlugin
        {
            DisplayName = "Oxygen",
            Url = "123",
            PluginId = 1,
            ProjectId = 1,
        };

        var pluginOne = await _repository.StoreProjectPlugin(pluginMethane);
        _ = await _context.SaveChangesAsync();
        var pluginTwo = await _repository.StoreProjectPlugin(pluginOxygen);
        _ = await _context.SaveChangesAsync();

        Assert.Multiple(() =>
        {
            Assert.That(pluginOne, Is.Not.Null);
            Assert.That(pluginTwo, Is.Not.Null);
        });

        Assert.That(pluginOne.Id, Is.Not.EqualTo(pluginTwo.Id));
    }

    [Test]
    public async Task StoreProjectPlugin_NoIdIncrementWhenIdExists_Test()
    {
        _context.Projects.Add(
            new Project
            {
                Id = 1,
                ProjectName = "A",
                Slug = "a",
                ClientName = "1",
                CompanyId = 1,
            }
        );
        _context.Plugins.Add(
            new Plugin
            {
                PluginName = "Warp-Drive",
                ProjectPlugins = [],
                Id = 3,
            }
        );
        await _context.SaveChangesAsync();
        var exampleProjectPlugin = new ProjectPlugin
        {
            DisplayName = "Warp-Drive",
            PluginId = 3,
            Url = "134",
            Id = 42,
            ProjectId = 1,
        };
        _ = _context.Add(exampleProjectPlugin);
        _ = await _context.SaveChangesAsync();

        exampleProjectPlugin.DisplayName = "Hall Effect Thruster";

        var plugin = await _repository.StoreProjectPlugin(exampleProjectPlugin);

        Assert.That(plugin, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(plugin.DisplayName, Is.EqualTo("Hall Effect Thruster"));
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
        _ = _context.Add(examplePlugin);
        _ = await _context.SaveChangesAsync();

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
        _ = Assert.ThrowsAsync<PluginNotFoundException>(() => _repository.GetPluginByIdAsync(42));
    }

    [Test]
    public async Task GetProjectPluginById_Test()
    {
        _context.Projects.Add(
            new Project
            {
                Id = 3,
                ProjectName = "A",
                Slug = "a",
                ClientName = "1",
                CompanyId = 1,
            }
        );
        _context.Plugins.Add(
            new Plugin
            {
                PluginName = "Warp-Drive",
                ProjectPlugins = [],
                Id = 3,
            }
        );
        await _context.SaveChangesAsync();
        var examplePlugin = new ProjectPlugin
        {
            DisplayName = "Warp-Drive",
            Url = "Url",
            PluginId = 3,
            ProjectId = 3,
            Id = 42,
        };
        _ = _context.Add(examplePlugin);
        _ = await _context.SaveChangesAsync();

        var plugin = await _repository.GetProjectPluginAsync(3, 42);

        Assert.That(plugin, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(plugin.DisplayName, Is.EqualTo("Warp-Drive"));
            Assert.That(plugin.Id, Is.EqualTo(42));
        });
    }

    [Test]
    public void GetProjectPluginById_NotFound_Test()
    {
        _ = Assert.ThrowsAsync<ProjectPluginNotFoundException>(() =>
            _repository.GetProjectPluginAsync(42, 3)
        );
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
        _ = _context.Add(examplePlugin);
        _ = await _context.SaveChangesAsync();

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
            CompanyId = 1,
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
        var projectPluginRelation1 = new ProjectPlugin
        {
            ProjectId = 1,
            PluginId = 1,
            Id = 2,
            Plugin = unarchivedPlugin,
            Project = project,
            Url = "unarchived.com",
        };
        var projectPluginRelation2 = new ProjectPlugin
        {
            ProjectId = 1,
            PluginId = 2,
            Id = 1,
            Plugin = archivedPlugin,
            Project = project,
            Url = "archived.com",
        };
        _ = _context.Projects.Add(project);
        _context.Plugins.AddRange(unarchivedPlugin, archivedPlugin);
        _context.ProjectPluginsRelation.AddRange(projectPluginRelation1, projectPluginRelation2);
        _ = await _context.SaveChangesAsync();

        var result = await (
            await _repository.GetAllUnarchivedPluginsForProjectIdAsync(1)
        ).ToListAsync();

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
            CompanyId = 1,
        };
        var archivedPlugin = new Plugin
        {
            Id = 1,
            PluginName = "Archived Plugin",
            IsArchived = true,
        };
        var projectPluginRelation = new ProjectPlugin
        {
            ProjectId = 1,
            PluginId = 1,
            Plugin = archivedPlugin,
            Project = project,
            Url = "archived.com",
        };
        _ = _context.Projects.Add(project);
        _ = _context.Plugins.Add(archivedPlugin);
        _ = _context.ProjectPluginsRelation.Add(projectPluginRelation);
        _ = await _context.SaveChangesAsync();

        var result = await (
            await _repository.GetAllUnarchivedPluginsForProjectIdAsync(1)
        ).ToListAsync();

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
            CompanyId = 1,
        };
        _ = _context.Projects.Add(project);
        _ = await _context.SaveChangesAsync();

        var result = await (
            await _repository.GetAllUnarchivedPluginsForProjectIdAsync(1)
        ).ToListAsync();

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
            CompanyId = 1,
        };
        var archivedPlugin = new Plugin
        {
            Id = 1,
            PluginName = "Archived Plugin",
            IsArchived = true,
        };
        var projectPluginRelation = new ProjectPlugin
        {
            ProjectId = 1,
            PluginId = 1,
            Plugin = archivedPlugin,
            Project = project,
            Url = "archived.com",
        };
        _ = _context.Projects.Add(project);
        _ = _context.Plugins.Add(archivedPlugin);
        _ = _context.ProjectPluginsRelation.Add(projectPluginRelation);
        _ = await _context.SaveChangesAsync();

        // Act
        var result = await (
            await _repository.GetAllUnarchivedPluginsForProjectIdAsync(1)
        ).ToListAsync();

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
            CompanyId = 1,
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
        var projectPluginRelation1 = new ProjectPlugin
        {
            ProjectId = 1,
            PluginId = 1,
            Id = 6,
            Plugin = unarchivedPlugin,
            Project = project,
            Url = "unarchived.com",
        };
        var projectPluginRelation2 = new ProjectPlugin
        {
            ProjectId = 1,
            PluginId = 2,
            Id = 99,
            Plugin = archivedPlugin,
            Project = project,
            Url = "archived.com",
        };
        _ = _context.Projects.Add(project);
        _context.Plugins.AddRange(unarchivedPlugin, archivedPlugin);
        _context.ProjectPluginsRelation.AddRange(projectPluginRelation1, projectPluginRelation2);
        _ = await _context.SaveChangesAsync();

        var result = await (
            await _repository.GetAllUnarchivedPluginsForProjectIdAsync(1)
        ).ToListAsync();

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
            CompanyId = 1,
        };
        var project2 = new Project
        {
            Id = 2,
            ProjectName = "Test Project2",
            Slug = "test_project2",
            ClientName = "Test Client2", // Make sure this is set
            CompanyId = 1,
        };
        var unarchivedPlugin = new Plugin
        {
            Id = 1,
            PluginName = "Unarchived Plugin",
            IsArchived = false,
        };

        var projectPluginRelation1 = new ProjectPlugin
        {
            ProjectId = 1,
            PluginId = 1,
            Plugin = unarchivedPlugin,
            Project = project1,
            Url = "plugin1.com",
        };
        var projectPluginRelation2 = new ProjectPlugin
        {
            ProjectId = 2,
            PluginId = 1,
            Plugin = unarchivedPlugin,
            Project = project2,
            Url = "plugin2.com",
        };

        _context.Projects.AddRange(project1, project2);
        _ = _context.Plugins.Add(unarchivedPlugin);
        _context.ProjectPluginsRelation.AddRange(projectPluginRelation1, projectPluginRelation2);
        _ = await _context.SaveChangesAsync();

        var result = await (
            await _repository.GetAllUnarchivedPluginsForProjectIdAsync(1)
        ).ToListAsync();

        Assert.That(result, Has.Count.EqualTo(1)); // Only the plugin for project 1 should be returned
        Assert.That(result[0].Plugin?.PluginName, Is.EqualTo("Unarchived Plugin"));
    }

    [Test]
    public void TestGetPluginsForNonExistentProjectThrowsException()
    {
        const int nonExistentProjectId = 999;

        var ex = Assert.ThrowsAsync<ProjectNotFoundException>(async () =>
        {
            _ = await _repository.GetAllUnarchivedPluginsForProjectIdAsync(nonExistentProjectId);
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
            CompanyId = 1,
        };
        var project2 = new Project
        {
            Id = 2,
            ProjectName = "Test Project2",
            ClientName = "Test Client2",
            Slug = "testProject2",
            CompanyId = 1,
        };
        var archivedPlugin = new Plugin
        {
            Id = 1,
            PluginName = "Unarchived Plugin",
            IsArchived = true,
        };

        var projectPluginRelation1 = new ProjectPlugin
        {
            ProjectId = 1,
            PluginId = 1,
            Plugin = archivedPlugin,
            Project = project1,
            Url = "plugin1.com",
        };
        var projectPluginRelation2 = new ProjectPlugin
        {
            ProjectId = 2,
            PluginId = 1,
            Plugin = archivedPlugin,
            Project = project2,
            Url = "plugin2.com",
        };

        _context.Projects.AddRange(project1, project2);
        _ = _context.Plugins.Add(archivedPlugin);
        _context.ProjectPluginsRelation.AddRange(projectPluginRelation1, projectPluginRelation2);

        _ = await _context.SaveChangesAsync();

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
    public async Task TestDeleteProjectPlugins()
    {
        // Arrange
        var project1 = new Project
        {
            Id = 1,
            ProjectName = "Test Project",
            ClientName = "Test Client",
            Slug = "testProject",
            CompanyId = 1,
        };

        var archivedPlugin = new Plugin
        {
            Id = 1,
            PluginName = "Unarchived Plugin",
            IsArchived = true,
        };

        var projectPluginRelation1 = new ProjectPlugin
        {
            ProjectId = 2,
            PluginId = 1,
            Plugin = archivedPlugin,
            Project = project1,
            Url = "plugin2.com",
        };

        _context.Projects.AddRange(project1);
        _ = _context.Plugins.Add(archivedPlugin);
        _context.ProjectPluginsRelation.AddRange(projectPluginRelation1);

        _ = await _context.SaveChangesAsync();

        // Act
        var returnValDeleteProjectPlugin = await _repository.DeleteProjectPlugin(
            projectPluginRelation1
        );

        // Assert
        Assert.That(returnValDeleteProjectPlugin, Is.True);

        _context.Entry(project1).State = EntityState.Detached;

        var reloadedProject1 = await _context
            .Projects.Include(p => p.ProjectPlugins)
            .FirstOrDefaultAsync(p => p.Id == 1);

        Assert.Multiple(() =>
        {
            Assert.That(reloadedProject1, Is.Not.Null);
            Assert.That(reloadedProject1?.ProjectPlugins, Is.Empty);
        });
    }

    [Test]
    public async Task CheckPluginNameExists_Test()
    {
        var plugin = new Plugin { Id = 1, PluginName = "Gitlab" };
        _ = _context.Plugins.Add(plugin);

        _ = await _context.SaveChangesAsync();

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
        _ = _context.Plugins.Add(plugin);

        _ = await _context.SaveChangesAsync();

        var result = await _repository.CheckGlobalPluginNameExists("gitLaB");

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckProjectPluginExists_Test()
    {
        _context.Projects.Add(
            new Project
            {
                Id = 1,
                ProjectName = "A",
                Slug = "a",
                ClientName = "1",
                CompanyId = 1,
            }
        );
        _context.Plugins.Add(
            new Plugin
            {
                PluginName = "Warp-Drive",
                ProjectPlugins = [],
                Id = 1,
            }
        );
        var plugin = new ProjectPlugin
        {
            Id = 1,
            ProjectId = 1,
            PluginId = 1,
            DisplayName = "Gitlab",
            Url = "gitlab.de",
        };
        _ = _context.ProjectPluginsRelation.Add(plugin);

        _ = await _context.SaveChangesAsync();

        var result = await _repository.CheckProjectPluginExists(1, 1, "gitlab.de");

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckProjectPluginExists_Not_Test()
    {
        var result = await _repository.CheckProjectPluginExists(1, 1, "Bielefeld");

        Assert.That(result, Is.False);
    }
}
