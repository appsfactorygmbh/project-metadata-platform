using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
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

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    [Test]
    public async Task CreateProject_Test()
    {
        var exampleProject = new Project
        {
            ProjectName = "Example Project",
            Slug = "example_project",
            ClientName = "Example Client",
            CompanyId = 1,
        };
        await _repository.AddProjectAsync(exampleProject);
        _ = await _context.SaveChangesAsync();
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
            CompanyId = 1,
        };
        await _repository.AddProjectAsync(exampleProject);
        _ = await _context.SaveChangesAsync();
        var firstResult = await _repository.GetProjectsAsync();
        await _repository.AddProjectAsync(exampleProject);
        _ = await _context.SaveChangesAsync();

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
}
