using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Companies;
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

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    private ProjectMetadataPlatformDbContext _context;
    private ProjectsRepository _repository;

    [Test]
    public async Task GetProjectsWithSearchTest()
    {
        // Arrange
        _context.Companies.Add(new Company { Id = 1, CompanyName = "A" });
        var exampleProject = new Project
        {
            Id = 1,
            ProjectName = "Regen",
            Slug = "regen",
            ClientName = "Nasa",
            CompanyId = 1,
        };

        var query = new GetAllProjectsQuery(null, "Reg", null, null);

        _ = _context.Projects.Add(exampleProject);
        _ = await _context.SaveChangesAsync();
        _ = await _context.Database.ExecuteSqlRawAsync(
            "REFRESH MATERIALIZED VIEW CONCURRENTLY mv_project_search;"
        );
        // Act
        var result = (await _repository.GetProjectsAsync(query, null)).ToList();

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
        _context.Companies.Add(new Company { Id = 1, CompanyName = "A" });
        var project = new Project
        {
            Id = 1,
            ProjectName = "Regen",
            Slug = "regen",
            ClientName = "Nasa",
            CompanyId = 1,
            CompanyState = CompanyState.INTERNAL,
        };

        var query = new GetAllProjectsQuery(null, "x", null, null);

        _ = _context.Projects.Add(project);
        _ = await _context.SaveChangesAsync();
        _ = await _context.Database.ExecuteSqlRawAsync(
            "REFRESH MATERIALIZED VIEW CONCURRENTLY mv_project_search;"
        );
        var result = await _repository.GetProjectsAsync(query, null);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetProjectsWithSearch_MultipleMatches_Test()
    {
        // Arrange
        _context.Companies.Add(new Company { Id = 1, CompanyName = "A" });
        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                ProjectName = "Wasserfall",
                Slug = "wasserfall",
                ClientName = "whatever_taucht_nicht_auf",
                CompanyId = 1,
            },
            new()
            {
                Id = 2,
                ProjectName = "Regen",
                Slug = "regen",
                ClientName = "ESA",
                CompanyId = 1,
            },
            new()
            {
                Id = 3,
                ProjectName = "Turbo",
                Slug = "turbo",
                ClientName = "Regen",
                CompanyId = 1,
            },
        };

        var query = new GetAllProjectsQuery(null, "Reg", null, null);

        _context.Projects.AddRange(projects);

        _ = await _context.SaveChangesAsync();
        _ = await _context.Database.ExecuteSqlRawAsync(
            "REFRESH MATERIALIZED VIEW CONCURRENTLY mv_project_search;"
        );

        // Act
        var result = await _repository.GetProjectsAsync(query, null);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetProjectsWithSearch_IgnoreCase_Test()
    {
        // Arrange
        _context.Companies.Add(new Company { Id = 1, CompanyName = "A" });
        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                ProjectName = "Wasserfall",
                Slug = "wasserfall",
                ClientName = "whatever_taucht_nicht_auf",
                CompanyId = 1,
            },
            new()
            {
                Id = 2,
                ProjectName = "Regen",
                Slug = "regen",
                ClientName = "ESA",
                CompanyId = 1,
            },
            new()
            {
                Id = 3,
                ProjectName = "Turbo",
                Slug = "turbo",
                ClientName = "Regen",
                CompanyId = 1,
            },
        };

        var query1 = new GetAllProjectsQuery(null, "Reg", null, null);
        var query2 = new GetAllProjectsQuery(null, "EGen", null, null);

        _context.Projects.AddRange(projects);

        _ = await _context.SaveChangesAsync();
        _ = await _context.Database.ExecuteSqlRawAsync(
            "REFRESH MATERIALIZED VIEW CONCURRENTLY mv_project_search;"
        );
        // Act
        var result = await _repository.GetProjectsAsync(query1, null);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));

        var resultIgnoreCase = await _repository.GetProjectsAsync(query2, null);

        // Assert
        Assert.That(resultIgnoreCase.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetProjectsWithTeamNumber_Test()
    {
        // Arrange
        _context.Companies.Add(new Company { Id = 1, CompanyName = "A" });
        _context.BusinessUnits.Add(new BusinessUnit { Id = 1, BusinessUnitName = "A" });
        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                ProjectName = "Wasserfall",
                Slug = "wasserfall",
                ClientName = "whatever_taucht_nicht_auf",
                Team = new() { TeamName = "41", BusinessUnitId = 1 },
                CompanyId = 1,
            },
            new()
            {
                Id = 2,
                ProjectName = "Regen",
                Slug = "regen",
                ClientName = "ESA",
                Team = new() { TeamName = "42", BusinessUnitId = 1 },
                CompanyId = 1,
            },
            new()
            {
                Id = 3,
                ProjectName = "Turbo",
                Slug = "turbo",
                ClientName = "Regen",
                Team = new()
                {
                    TeamName = "422",

                    BusinessUnitId = 1,
                },
                CompanyId = 1,
            },
        };

        var query1 = new GetAllProjectsQuery(null, "42", null, null);
        var query2 = new GetAllProjectsQuery(null, "41", null, null);
        var query3 = new GetAllProjectsQuery(null, "4", null, null);
        var query4 = new GetAllProjectsQuery(null, "2", null, null);

        _context.Projects.AddRange(projects);

        _ = await _context.SaveChangesAsync();
        _ = await _context.Database.ExecuteSqlRawAsync(
            "REFRESH MATERIALIZED VIEW CONCURRENTLY mv_project_search;"
        );
        // Act
        var resultExactDouble = (await _repository.GetProjectsAsync(query1, null)).ToList();

        // Assert
        Assert.That(resultExactDouble.Count(), Is.EqualTo(2));

        var resultExactSingle = await _repository.GetProjectsAsync(query2, null);
        // Assert
        Assert.That(resultExactSingle.Count(), Is.EqualTo(1));

        var resultOnlyFirstNumber = await _repository.GetProjectsAsync(query3, null);
        // Assert
        Assert.That(resultOnlyFirstNumber.Count(), Is.EqualTo(3));

        var resultOnlySecondNumberNumber = await _repository.GetProjectsAsync(query4, null);
        // Assert
        Assert.That(resultOnlySecondNumberNumber.Count(), Is.EqualTo(2));
    }
}
