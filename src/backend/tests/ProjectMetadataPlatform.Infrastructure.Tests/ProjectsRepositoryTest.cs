using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Infrastructure.Projects;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

[TestFixture]
public class ProjectsRepositoryTests : TestsWithDatabase
{
    [SetUp]
    public void Setup()
    {
        _context = DbContext();
        _repository = new ProjectsRepository(_context);
        ClearData(_context);
    }

    private ProjectMetadataPlatformDbContext _context;
    private ProjectsRepository _repository;

    [Test]
    public async Task GetAllProjectsAsync_ShouldReturnAllProjects()
    {
        // Arrange
        var project = new Project
        {
            Id = 1,
            ProjectName = "Regen",
            Slug = "regen",
            ClientName = "Nasa",
        };

        _context.Projects.RemoveRange(_context.Projects);
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetProjectsAsync()).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        project = result.First();
        Assert.Multiple(() =>
        {
            Assert.That(project.Id, Is.EqualTo(1));
            Assert.That(project.ProjectName, Is.EqualTo("Regen"));
            Assert.That(project.ClientName, Is.EqualTo("Nasa"));
        });
    }

    [Test]
    public async Task GetProjectByMultipleFiltersAndSearchAsync_ReturnsCorrectProjects()
    {
        var filters = new ProjectFilterRequest(
            "Heather",
            "Metatron",
            ["666", "777"],
            ["42", "43"],
            true,
            ["AppsFact"],
            SecurityLevel.VERY_HIGH
        );
        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                ProjectName = "Heather",
                Slug = "heather",
                ClientName = "Metatron",
                IsArchived = true,
                Company = "AppsFact",
                IsmsLevel = SecurityLevel.VERY_HIGH,
                Team = new() { TeamName = "42", BusinessUnit = "666" },
            },
            new()
            {
                Id = 2,
                ProjectName = "James",
                Slug = "james",
                ClientName = "Lucifer",
                IsArchived = true,
                Company = "AppsFact",
                IsmsLevel = SecurityLevel.VERY_HIGH,
                Team = new() { TeamName = "43", BusinessUnit = "777" },
            },
            new()
            {
                Id = 3,
                ProjectName = "Marika",
                Slug = "marika",
                ClientName = "Satan",
                IsArchived = false,
                Company = "AppsFact",
                IsmsLevel = SecurityLevel.VERY_HIGH,
                Team = new() { TeamName = "44", BusinessUnit = "999" },
            },
        };

        var query = new GetAllProjectsQuery(filters, "Hea");

        await _context.Database.EnsureCreatedAsync();
        _context.Projects.AddRange(projects);
        await _context.SaveChangesAsync();

        var result = (await _repository.GetProjectsAsync(query)).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(result.Any(p => p.ProjectName == "Heather"), Is.True);
            Assert.That(result.Any(p => p.ClientName == "Metatron"), Is.True);
            Assert.That(result.Any(p => p.IsArchived), Is.True);
            Assert.That(result.Any(p => p.Company == "AppsFact"), Is.True);
            Assert.That(result.Any(p => p.IsmsLevel == SecurityLevel.VERY_HIGH), Is.True);
        });
    }

    [Test]
    public async Task GetProjectsByFiltersAsync_NoMatchingProjects_ReturnsEmpty()
    {
        var filters = new ProjectFilterRequest(
            "Heather",
            "Gilgamesch",
            ["666", "777"],
            ["42", "43"],
            null,
            ["Nothing else"],
            null
        );
        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                ProjectName = "Heather",
                Slug = "heather",
                ClientName = "Metatron",
                Company = "AddOn",
                IsmsLevel = SecurityLevel.HIGH,
            },
            new()
            {
                Id = 2,
                ProjectName = "James",
                Slug = "james",
                ClientName = "Lucifer",
                Company = "AddOn",
                IsmsLevel = SecurityLevel.HIGH,
            },
        };

        var query = new GetAllProjectsQuery(filters, null);

        await _context.Database.EnsureCreatedAsync();
        _context.Projects.AddRange(projects);
        await _context.SaveChangesAsync();

        var result = await _repository.GetProjectsAsync(query);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetProjectsByFiltersAsync_NoFilters_ReturnsAllProjects()
    {
        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                ProjectName = "Heather",
                Slug = "heather",
                ClientName = "Metatron",
            },
            new()
            {
                Id = 2,
                ProjectName = "James",
                Slug = "james",
                ClientName = "Lucifer",
            },
            new()
            {
                Id = 3,
                ProjectName = "Marika",
                Slug = "marika",
                ClientName = "Satan",
            },
        };

        var query = new GetAllProjectsQuery(null, null);

        await _context.Database.EnsureCreatedAsync();
        _context.Projects.AddRange(projects);
        await _context.SaveChangesAsync();

        var result = await _repository.GetProjectsAsync(query);

        Assert.That(result.Count(), Is.EqualTo(3));
    }

    [Test]
    public async Task DeleteProjectAsync_ShouldDeleteProject()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Regen",
            Slug = "regen",
            ClientName = "Nasa",
        };

        _context.Projects.RemoveRange(_context.Projects);
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var deletedProject = await _repository.DeleteProjectAsync(project);
        await _context.SaveChangesAsync();
        var remainingProjects = await _context.Projects.ToListAsync();

        Assert.Multiple(() =>
        {
            Assert.That(remainingProjects, Is.Empty);
            Assert.That(deletedProject, Is.EqualTo(project));
        });
    }

    [Test]
    public async Task GetProjectBySlug_Test()
    {
        var project1 = new Project
        {
            Id = 1,
            ProjectName = "Regen",
            Slug = "regen",
            ClientName = "Nasa",
        };

        var project2 = new Project
        {
            Id = 2,
            ProjectName = "Nieselegen",
            Slug = "nieselregen",
            ClientName = "Nasa",
        };

        _context.Projects.RemoveRange(_context.Projects);
        _context.Projects.Add(project1);
        _context.Projects.Add(project2);
        await _context.SaveChangesAsync();

        var result = await _repository.GetProjectIdBySlugAsync("regen");

        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public async Task GetProjectBySlug_Test_NotFoundReturnsNull()
    {
        var project2 = new Project
        {
            Id = 2,
            ProjectName = "Nieselegen",
            Slug = "nieselregen",
            ClientName = "Nasa",
        };

        _context.Projects.RemoveRange(_context.Projects);
        _context.Projects.Add(project2);
        await _context.SaveChangesAsync();

        Assert.ThrowsAsync<ProjectNotFoundException>(() =>
            _repository.GetProjectIdBySlugAsync("regen")
        );
    }
}
