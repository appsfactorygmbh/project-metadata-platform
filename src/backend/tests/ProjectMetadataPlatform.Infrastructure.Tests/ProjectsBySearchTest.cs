using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Infrastructure.Projects;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

[TestFixture]
public class ProjectsBySearchTest : TestsWithDatabase
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
    public async Task GetProjectsWithSearchTest()
    {
        // Arrange
        var exampleProject = new Project
        {
            Id = 1,
            ProjectName = "Regen",
            Slug = "regen",
            ClientName = "Nasa",
        };

        var query = new GetAllProjectsQuery(null, "Reg");

        _context.Projects.Add(exampleProject);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetProjectsAsync(query)).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        var project = result.First();
        Assert.Multiple(() =>
        {
            Assert.That(project.Id, Is.EqualTo(1));
            Assert.That(project.ProjectName, Is.EqualTo("Regen"));
            Assert.That(project.ClientName, Is.EqualTo("Nasa"));
        });
    }

    [Test]
    public async Task GetProjectsWithSearch_WithoutMatch_Test()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Regen",
            Slug = "regen",
            ClientName = "Nasa",
        };

        var query = new GetAllProjectsQuery(null, "x");

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var result = await _repository.GetProjectsAsync(query);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetProjectsWithSearch_MultipleMatches_Test()
    {
        // Arrange
        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                ProjectName = "Wasserfall",
                Slug = "wasserfall",
                ClientName = "whatever_taucht_nicht_auf",
            },
            new()
            {
                Id = 2,
                ProjectName = "Regen",
                Slug = "regen",
                ClientName = "ESA",
            },
            new()
            {
                Id = 3,
                ProjectName = "Turbo",
                Slug = "turbo",
                ClientName = "Regen",
            },
        };

        var query = new GetAllProjectsQuery(null, "Reg");

        _context.Projects.AddRange(projects);

        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetProjectsAsync(query);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetProjectsWithSearch_IgnoreCase_Test()
    {
        // Arrange
        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                ProjectName = "Wasserfall",
                Slug = "wasserfall",
                ClientName = "whatever_taucht_nicht_auf",
            },
            new()
            {
                Id = 2,
                ProjectName = "Regen",
                Slug = "regen",
                ClientName = "ESA",
            },
            new()
            {
                Id = 3,
                ProjectName = "Turbo",
                Slug = "turbo",
                ClientName = "Regen",
            },
        };

        var query1 = new GetAllProjectsQuery(null, "Reg");
        var query2 = new GetAllProjectsQuery(null, "EGen");

        _context.Projects.AddRange(projects);

        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetProjectsAsync(query1);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));

        var resultIgnoreCase = await _repository.GetProjectsAsync(query2);

        // Assert
        Assert.That(resultIgnoreCase.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetProjectsWithTeamNumber_Test()
    {
        // Arrange
        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                ProjectName = "Wasserfall",
                Slug = "wasserfall",
                ClientName = "whatever_taucht_nicht_auf",
                Team = new() { TeamName = "41", BusinessUnit = "BU Test" },
            },
            new()
            {
                Id = 2,
                ProjectName = "Regen",
                Slug = "regen",
                ClientName = "ESA",
                Team = new() { TeamName = "42", BusinessUnit = "BU Test" },
            },
            new()
            {
                Id = 3,
                ProjectName = "Turbo",
                Slug = "turbo",
                ClientName = "Regen",
                Team = new() { TeamName = "422", BusinessUnit = "BU Test" },
            },
        };

        var query1 = new GetAllProjectsQuery(null, "42");
        var query2 = new GetAllProjectsQuery(null, "41");
        var query3 = new GetAllProjectsQuery(null, "4");
        var query4 = new GetAllProjectsQuery(null, "2");

        _context.Projects.AddRange(projects);

        await _context.SaveChangesAsync();

        // Act
        var resultExactDouble = await _repository.GetProjectsAsync(query1);

        // Assert
        Assert.That(resultExactDouble.Count(), Is.EqualTo(2));

        var resultExactSingle = await _repository.GetProjectsAsync(query2);
        // Assert
        Assert.That(resultExactSingle.Count(), Is.EqualTo(1));

        var resultOnlyFirstNumber = await _repository.GetProjectsAsync(query3);
        // Assert
        Assert.That(resultOnlyFirstNumber.Count(), Is.EqualTo(3));

        var resultOnlySecondNumberNumber = await _repository.GetProjectsAsync(query4);
        // Assert
        Assert.That(resultOnlySecondNumberNumber.Count(), Is.EqualTo(2));
    }
}
