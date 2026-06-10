using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.BusinessUnits;
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

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
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
            CompanyId = 1,
        };

        _context.Projects.RemoveRange(_context.Projects);
        _ = _context.Projects.Add(project);
        _ = await _context.SaveChangesAsync();

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
            ["Test", "Test2"],
            ["42", "43"],
            true,
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
                Company = new() { CompanyName = "AppsFact" },
                CompanyId = 1,
                IsEoC = true,
                IsmsLevel = SecurityLevel.VERY_HIGH,
                Team = new()
                {
                    TeamName = "42",
                    BusinessUnitId = 666,
                    BusinessUnit = new BusinessUnit { BusinessUnitName = "Test" },
                },
            },
            new()
            {
                Id = 2,
                ProjectName = "James",
                Slug = "james",
                ClientName = "Lucifer",
                IsArchived = true,
                IsmsLevel = SecurityLevel.VERY_HIGH,
                Team = new()
                {
                    TeamName = "43",
                    BusinessUnitId = 777,
                    BusinessUnit = new BusinessUnit { BusinessUnitName = "Test2" },
                },
                CompanyId = 1,
            },
            new()
            {
                Id = 3,
                ProjectName = "Marika",
                Slug = "marika",
                ClientName = "Satan",
                IsArchived = false,
                IsmsLevel = SecurityLevel.VERY_HIGH,
                Team = new()
                {
                    TeamName = "44",
                    BusinessUnitId = 999,
                    BusinessUnit = new BusinessUnit { BusinessUnitName = "Test3" },
                },
                CompanyId = 1,
            },
        };

        var query = new GetAllProjectsQuery(filters, "Hea");

        _ = await _context.Database.EnsureCreatedAsync();
        _context.Projects.AddRange(projects);
        _ = await _context.SaveChangesAsync();

        var result = (await _repository.GetProjectsAsync(query)).ToList();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(result.Any(p => p.ProjectName == "Heather"), Is.True);
            Assert.That(result.Any(p => p.ClientName == "Metatron"), Is.True);
            Assert.That(result.Any(p => p.IsArchived), Is.True);
            Assert.That(result.Any(p => p.Company!.CompanyName == "AppsFact"), Is.True);
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
                Company = new() { CompanyName = "AddOn" },
                CompanyId = 1,
                IsmsLevel = SecurityLevel.HIGH,
            },
            new()
            {
                Id = 2,
                ProjectName = "James",
                Slug = "james",
                ClientName = "Lucifer",
                CompanyId = 1,
                IsmsLevel = SecurityLevel.HIGH,
            },
        };

        var query = new GetAllProjectsQuery(filters, null);

        _ = await _context.Database.EnsureCreatedAsync();
        _context.Projects.AddRange(projects);
        _ = await _context.SaveChangesAsync();

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
                CompanyId = 1,
            },
            new()
            {
                Id = 2,
                ProjectName = "James",
                Slug = "james",
                ClientName = "Lucifer",
                CompanyId = 1,
            },
            new()
            {
                Id = 3,
                ProjectName = "Marika",
                Slug = "marika",
                ClientName = "Satan",
                CompanyId = 1,
            },
        };

        var query = new GetAllProjectsQuery(null, null);

        _ = await _context.Database.EnsureCreatedAsync();
        _context.Projects.AddRange(projects);
        _ = await _context.SaveChangesAsync();

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
            CompanyId = 1,
        };

        _context.Projects.RemoveRange(_context.Projects);
        _ = _context.Projects.Add(project);
        _ = await _context.SaveChangesAsync();

        var deletedProject = await _repository.DeleteProjectAsync(project);
        _ = await _context.SaveChangesAsync();
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
            CompanyId = 1,
        };

        var project2 = new Project
        {
            Id = 2,
            ProjectName = "Nieselegen",
            Slug = "nieselregen",
            ClientName = "Nasa",
            CompanyId = 1,
        };

        _context.Projects.RemoveRange(_context.Projects);
        _ = _context.Projects.Add(project1);
        _ = _context.Projects.Add(project2);
        _ = await _context.SaveChangesAsync();

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
            CompanyId = 1,
        };

        _context.Projects.RemoveRange(_context.Projects);
        _ = _context.Projects.Add(project2);
        _ = await _context.SaveChangesAsync();

        _ = Assert.ThrowsAsync<ProjectNotFoundException>(() =>
            _repository.GetProjectIdBySlugAsync("regen")
        );
    }
}
