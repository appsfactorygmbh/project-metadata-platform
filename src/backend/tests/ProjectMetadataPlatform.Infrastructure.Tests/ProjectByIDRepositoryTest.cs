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

    [Test]
    public void GetProjectByIDAsync_NonexistentProject()
    {
        Assert.ThrowsAsync<ProjectNotFoundException>(() => _repository.GetProjectAsync(1));
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
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

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
