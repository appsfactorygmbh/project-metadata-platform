using System.Threading.Tasks;
using NUnit.Framework;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Infrastructure.Projects;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

public class ProjectByIdRepositoryTest : TestsWithDatabase
{
    private ProjectMetadataPlatformDbContext _context;
    private ProjectsRepository _repository;

    [SetUp]
    public void Setup()
    {
        _context = DbContext();
        _repository = new ProjectsRepository(_context);
        ClearData(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    [Test]
    public void GetProjectByIDAsync_NonexistentProject()
    {
        _ = Assert.ThrowsAsync<ProjectNotFoundException>(() => _repository.GetProjectAsync(1));
    }

    [Test]
    public async Task GetProjectByIDAsync_ReturnProject()
    {
        // Arrange
        var project = new Project
        {
            Id = 1,
            ProjectName = "Regen",
            Slug = "regen",
            ClientName = "Nasa",
            CompanyId = 1,
        };

        _ = _context.Projects.Add(project);
        _ = await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetProjectAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.ProjectName, Is.EqualTo("Regen"));
            Assert.That(result.ClientName, Is.EqualTo("Nasa"));
        });
    }
}
