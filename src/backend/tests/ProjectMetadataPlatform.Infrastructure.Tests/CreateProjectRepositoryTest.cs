using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Infrastructure.Plugins;
using ProjectMetadataPlatform.Infrastructure.Projects;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

[TestFixture]
public class CreateProjectRepositoryTest : TestsWithDatabase
{
    private ProjectMetadataPlatformDbContext _context;
    private ProjectsRepository _repository;
    private PluginRepository _pluginRepository;

    [SetUp]
    public void Setup()
    {
        _context = DbContext();
        _repository = new ProjectsRepository(_context);
        _pluginRepository = new PluginRepository(_context);
        ClearData(_context);
    }

    [Test]
    public async Task CreateProject_Test()
    {
        var exampleProject = new Project
        {
            ProjectName = "Example Project",
            Slug = "example_project",
            ClientName = "Example Client",
        };
        await _repository.AddProjectAsync(exampleProject);
        await _context.SaveChangesAsync();
        Assert.That(exampleProject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(exampleProject.ProjectName, Is.EqualTo("Example Project"));
            Assert.That(exampleProject.ClientName, Is.EqualTo("Example Client"));
            Assert.That(exampleProject.Id, Is.GreaterThan(0));
        });
    }

    [Test]
    public async Task CreateProject_ProjectAlreadyExists_Test()
    {
        var exampleProject = new Project
        {
            Id = 1,
            ProjectName = "Example Project",
            Slug = "example_project",
            ClientName = "Example Client",
        };
        await _repository.AddProjectAsync(exampleProject);
        await _context.SaveChangesAsync();
        var firstResult = await _repository.GetProjectsAsync();
        await _repository.AddProjectAsync(exampleProject);
        await _context.SaveChangesAsync();

        Assert.That(exampleProject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(exampleProject.ProjectName, Is.EqualTo("Example Project"));
            Assert.That(exampleProject.ClientName, Is.EqualTo("Example Client"));
            Assert.That(exampleProject.Id, Is.GreaterThan(0));
        });
        var result = await _repository.GetProjectsAsync();
        Assert.That(firstResult.Count(), Is.EqualTo(result.Count()));
    }

    [Test]
    public async Task CreateProjectWithPlugins_Test()
    {
        var projectPlugin = new ProjectPlugins
        {
            PluginId = 301,
            ProjectId = 1,
            Url = "dummy",
            DisplayName = "Dummy",
        };
        var projectPlugins = new List<ProjectPlugins> { projectPlugin };
        var exampleProject = new Project
        {
            ProjectName = "Example Project",
            Slug = "example_project",
            ClientName = "Example Client",
            ProjectPlugins = projectPlugins,
        };
        var examplePlugin = new Plugin { PluginName = "DummyPlug" };
        var storedPlugin = await _pluginRepository.StorePlugin(examplePlugin);
        await _repository.AddProjectAsync(exampleProject);
        await _context.SaveChangesAsync();
        var projectResult = _context.Projects.FirstOrDefault(p => p.Id == exampleProject.Id);
        Assert.That(projectResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(projectResult.ProjectName, Is.EqualTo("Example Project"));
            Assert.That(projectResult.ClientName, Is.EqualTo("Example Client"));
        });
        var pluginResult = _context
            .ProjectPluginsRelation.Where(p => p.ProjectId == exampleProject.Id)
            .Include(p => p.Plugin)
            .ToList();
        Assert.That(pluginResult, Is.Not.Null);
        Assert.That(pluginResult, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(pluginResult[0].Url, Is.EqualTo("dummy"));
            Assert.That(pluginResult[0].DisplayName, Is.EqualTo("Dummy"));
            Assert.That(pluginResult[0].Plugin!.PluginName, Is.EqualTo("DummyPlug"));
            Assert.That(pluginResult[0].PluginId, Is.EqualTo(storedPlugin.Id));
        });
    }
}
