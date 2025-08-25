using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Domain.Teams;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Infrastructure.Teams;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

[TestFixture]
public class TeamsRepositoryTests : TestsWithDatabase
{
    [SetUp]
    public void Setup()
    {
        _context = DbContext();
        _repository = new TeamRepository(_context);
        ClearData(_context);
    }

    private ProjectMetadataPlatformDbContext _context;
    private TeamRepository _repository;

    [Test]
    public async Task GetTeamsAsync_ShouldReturnAllTeams()
    {
        // Arrange
        var team = new Team
        {
            Id = 1,
            TeamName = "Test_1",
            BusinessUnit = "BU Test",
            PTL = "Max Mustermann",
        };

        var team2 = new Team
        {
            Id = 2,
            TeamName = "Test_2",
            BusinessUnit = "BU Foo",
        };

        _context.Teams.RemoveRange(_context.Teams);
        _context.Teams.Add(team);
        _context.Teams.Add(team2);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetTeamsAsync(null, null)).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        var teamRes = result.First();
        var teamRes2 = result.Last();
        Assert.Multiple(() =>
        {
            Assert.That(teamRes.Id, Is.EqualTo(1));
            Assert.That(teamRes.TeamName, Is.EqualTo("Test_1"));
            Assert.That(teamRes.PTL, Is.EqualTo("Max Mustermann"));
            Assert.That(teamRes.BusinessUnit, Is.EqualTo("BU Test"));
            Assert.That(teamRes2.Id, Is.EqualTo(2));
            Assert.That(teamRes2.TeamName, Is.EqualTo("Test_2"));
            Assert.That(teamRes2.PTL, Is.EqualTo(null));
            Assert.That(teamRes2.BusinessUnit, Is.EqualTo("BU Foo"));
        });
    }

    [TestCase("", 1, 2, 3)]
    [TestCase("Test", 1, 2)]
    [TestCase("foo bar", 3)]
    [TestCase("abc")]
    public async Task GetTeamByFilterTeamName_ReturnsCorrectProjects(
        string teamName,
        params int[] expectedIds
    )
    {
        // Arrange
        var goalIds = expectedIds.ToList();
        var teams = new List<Team>
        {
            new()
            {
                Id = 1,
                TeamName = "Test_1",
                BusinessUnit = "BU Test",
                PTL = "Max Mustermann",
            },
            new()
            {
                Id = 2,
                TeamName = "Test_2",
                BusinessUnit = "BU Foo",
                PTL = "Anna Mustermann",
            },
            new()
            {
                Id = 3,
                TeamName = "Foo Bar",
                BusinessUnit = "BU Bar",
                PTL = "Test",
            },
        };
        await _context.Database.EnsureCreatedAsync();
        _context.Teams.AddRange(teams);
        await _context.SaveChangesAsync();

        // Act
        var result = (
            await _repository.GetTeamsAsync(teamName: teamName, fullTextQuery: null)
        ).ToList();

        // Assert
        var ids = result.Select(team => team.Id).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(ids, Has.Count.EqualTo(goalIds.Count));
            Assert.That(goalIds, Is.EqualTo(ids));
        });
    }

    [TestCase("", 1, 2, 3)]
    [TestCase("foo bar", 3)]
    [TestCase("abc")]
    [TestCase("Test", 1, 2, 3)]
    [TestCase("bu", 1, 3)]
    [TestCase("muSTermann", 1, 2)]
    public async Task GetTeamByFullTextQuery_ReturnsCorrectProjects(
        string query,
        params int[] expectedIds
    )
    {
        // Arrange
        var goalIds = expectedIds.ToList();
        var teams = new List<Team>
        {
            new()
            {
                Id = 1,
                TeamName = "Test_1",
                BusinessUnit = "BU Test",
                PTL = "Max Mustermann",
            },
            new()
            {
                Id = 2,
                TeamName = "Test_2",
                BusinessUnit = "Foo",
                PTL = "Anna Mustermann",
            },
            new()
            {
                Id = 3,
                TeamName = "Foo Bar",
                BusinessUnit = "BU Bar",
                PTL = "Test",
            },
        };
        await _context.Database.EnsureCreatedAsync();
        _context.Teams.AddRange(teams);
        await _context.SaveChangesAsync();

        // Act
        var result = (
            await _repository.GetTeamsAsync(teamName: null, fullTextQuery: query)
        ).ToList();

        // Assert
        var ids = result.Select(team => team.Id).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(ids, Has.Count.EqualTo(goalIds.Count));
            Assert.That(goalIds, Is.EqualTo(ids));
        });
    }

    [TestCase("Test", "", 1, 2)]
    [TestCase("teSt", "Max", 1)]
    [TestCase("2", "Foo", 2)]
    [TestCase("Foo Bar", "2")]
    [TestCase("", "Bu", 1, 3)]
    public async Task GetTeamByFullTextQueryAndTeamName_ReturnsCorrectProjects(
        string teamName,
        string query,
        params int[] expectedIds
    )
    {
        // Arrange
        var goalIds = expectedIds.ToList();
        var teams = new List<Team>
        {
            new()
            {
                Id = 1,
                TeamName = "Test_1",
                BusinessUnit = "BU Test",
                PTL = "Max Mustermann",
            },
            new()
            {
                Id = 2,
                TeamName = "Test_2",
                BusinessUnit = "Foo",
                PTL = "Anna Mustermann",
            },
            new()
            {
                Id = 3,
                TeamName = "Foo Bar",
                BusinessUnit = "BU Bar",
                PTL = "Test",
            },
        };
        await _context.Database.EnsureCreatedAsync();
        _context.Teams.AddRange(teams);
        await _context.SaveChangesAsync();

        // Act
        var result = (
            await _repository.GetTeamsAsync(teamName: teamName, fullTextQuery: query)
        ).ToList();

        // Assert
        var ids = result.Select(team => team.Id).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(ids, Has.Count.EqualTo(goalIds.Count));
            Assert.That(goalIds, Is.EqualTo(ids));
        });
    }

    [Test]
    public async Task DeleteProjectAsync_ShouldDeleteProject()
    {
        // Arrange
        var team = new Team
        {
            Id = 1,
            TeamName = "Test_1",
            BusinessUnit = "BU Test",
            PTL = "Max Mustermann",
        };

        _context.Teams.RemoveRange(_context.Teams);
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        // Act
        var deletedTeam = await _repository.DeleteTeamAsync(team);
        await _context.SaveChangesAsync();

        // Assert
        var remainingTeams = _context.Teams.ToList();

        Assert.Multiple(() =>
        {
            Assert.That(remainingTeams, Is.Empty);
            Assert.That(deletedTeam, Is.EqualTo(team));
        });
    }

    [Test]
    public async Task GetTeamWithProjectsAsync_ShouldReturnTeamWithAssociatedProjects()
    {
        // Arrange
        var team = new Team
        {
            Id = 1,
            TeamName = "Test_1",
            BusinessUnit = "BU Test",
            PTL = "Max Mustermann",
        };

        var project = new Project
        {
            Id = 1,
            ProjectName = "Nieselegen",
            Slug = "nieselregen",
            ClientName = "Nasa",
            TeamId = 1,
        };

        _context.Teams.RemoveRange(_context.Teams);
        _context.Teams.Add(team);

        _context.Projects.RemoveRange(_context.Projects);
        _context.Projects.Add(project);

        await _context.SaveChangesAsync();

        // Act
        var teamWithProjects = await _repository.GetTeamWithProjectsAsync(1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(teamWithProjects.TeamName, Is.EqualTo("Test_1"));
            Assert.That(teamWithProjects.BusinessUnit, Is.EqualTo("BU Test"));
            Assert.That(teamWithProjects.PTL, Is.EqualTo("Max Mustermann"));
            Assert.That(teamWithProjects.Id, Is.EqualTo(1));

            Assert.That(teamWithProjects.Projects, Is.Not.Null);
            Assert.That(teamWithProjects.Projects, Has.Count.EqualTo(1));
            var linkedProject = teamWithProjects.Projects!.First();

            Assert.That(linkedProject.Id, Is.EqualTo(1));
            Assert.That(linkedProject.ProjectName, Is.EqualTo("Nieselegen"));
            Assert.That(linkedProject.Slug, Is.EqualTo("nieselregen"));
            Assert.That(linkedProject.ClientName, Is.EqualTo("Nasa"));
        });
    }

    [Test]
    public async Task GetTeamAsync_ShouldThrowTeamNotFoundExceptionIfNotPresent()
    {
        // Arrange
        _context.Teams.RemoveRange(_context.Teams);
        await _context.SaveChangesAsync();

        // Act + Assert
        var ex = Assert.ThrowsAsync<TeamNotFoundException>(async () =>
            await _repository.GetTeamAsync(1)
        );
        Assert.That(ex.Message, Does.Contain("1"));
    }

    [Test]
    public async Task RetrieveNameForIdAsync_ShouldReturnCorrectName()
    {
        // Arrange
        var team = new Team
        {
            Id = 1,
            TeamName = "Test_1",
            BusinessUnit = "BU Test",
            PTL = "Max Mustermann",
        };
        _context.Teams.RemoveRange(_context.Teams);
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        // Act
        var teamName = await _repository.RetrieveNameForIdAsync(1);

        // Assert
        Assert.That(teamName, Is.EqualTo("Test_1"));
    }

    [Test]
    public async Task AddTeamAsync_NewTeam_ShouldAddTeamToDatabase()
    {
        // Arrange
        var newTeam = new Team
        {
            Id = 100,
            TeamName = "New Team Alpha",
            BusinessUnit = "Innovation",
            PTL = "Dr. New",
        };

        // Act
        await _repository.AddTeamAsync(newTeam);
        await _context.SaveChangesAsync();

        // Assert
        var addedTeam = await _context.Teams.FindAsync(newTeam.Id);
        Assert.That(addedTeam, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(addedTeam.TeamName, Is.EqualTo(newTeam.TeamName));
            Assert.That(addedTeam.BusinessUnit, Is.EqualTo(newTeam.BusinessUnit));
            Assert.That(addedTeam.PTL, Is.EqualTo(newTeam.PTL));
        });
        Assert.That(await _context.Teams.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task AddTeamAsync_TeamWithExistingId_ShouldNotAddOrUpdateTeam()
    {
        // Arrange
        var initialTeam = new Team
        {
            Id = 101,
            TeamName = "Original Gamma",
            BusinessUnit = "Legacy",
            PTL = "Old Guard",
        };
        _context.Teams.Add(initialTeam);
        await _context.SaveChangesAsync();

        var teamWithSameId = new Team
        {
            Id = 101,
            TeamName = "Updated Gamma Attempt",
            BusinessUnit = "New Wave",
            PTL = "New Blood",
        };

        // Act
        await _repository.AddTeamAsync(teamWithSameId);
        await _context.SaveChangesAsync();

        // Assert
        var teamInDb = await _context.Teams.FindAsync(initialTeam.Id);
        Assert.That(teamInDb, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(teamInDb.TeamName, Is.EqualTo(initialTeam.TeamName));
            Assert.That(teamInDb.BusinessUnit, Is.EqualTo(initialTeam.BusinessUnit));
            Assert.That(teamInDb.PTL, Is.EqualTo(initialTeam.PTL));
        });
        Assert.That(await _context.Teams.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task CheckIfTeamNameExistsAsync_NameExists_ShouldReturnTrue()
    {
        // Arrange
        var existingTeamName = "Unique Existent Team";
        _context.Teams.Add(
            new Team
            {
                Id = 200,
                TeamName = existingTeamName,
                BusinessUnit = "BU",
                PTL = "PTL",
            }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.CheckIfTeamNameExistsAsync(existingTeamName);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckIfTeamNameExistsAsync_NameDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentTeamName = "Definitely Not Here Team";
        _context.Teams.Add(
            new Team
            {
                Id = 201,
                TeamName = "Some Other Team",
                BusinessUnit = "BU",
                PTL = "PTL",
            }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.CheckIfTeamNameExistsAsync(nonExistentTeamName);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CheckIfTeamNameExistsAsync_EmptyDatabase_ShouldReturnFalse()
    {
        // Arrange

        // Act
        var result = await _repository.CheckIfTeamNameExistsAsync("Any Name");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task UpdateTeamAsync_ExistingTeam_ShouldUpdateTeamProperties()
    {
        // Arrange
        var initialTeam = new Team
        {
            Id = 300,
            TeamName = "Team Epsilon",
            BusinessUnit = "Old BU",
            PTL = "Old PTL",
        };
        _context.Teams.Add(initialTeam);
        await _context.SaveChangesAsync();
        _context.Entry(initialTeam).State = EntityState.Detached;

        var updatedTeamData = new Team
        {
            Id = 300,
            TeamName = "Team Epsilon Updated",
            BusinessUnit = "New BU",
            PTL = "New PTL",
        };

        // Act
        var result = await _repository.UpdateTeamAsync(updatedTeamData);
        await _context.SaveChangesAsync();

        // Assert
        var teamFromDb = await _context.Teams.FindAsync(initialTeam.Id);
        Assert.That(teamFromDb, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(teamFromDb.TeamName, Is.EqualTo(updatedTeamData.TeamName));
            Assert.That(teamFromDb.BusinessUnit, Is.EqualTo(updatedTeamData.BusinessUnit));
            Assert.That(teamFromDb.PTL, Is.EqualTo(updatedTeamData.PTL));
            Assert.That(result.TeamName, Is.EqualTo(updatedTeamData.TeamName));
            Assert.That(result.BusinessUnit, Is.EqualTo(updatedTeamData.BusinessUnit));
            Assert.That(result.PTL, Is.EqualTo(updatedTeamData.PTL));
        });
    }

    [Test]
    public void UpdateTeamAsync_NonExistingTeam_ShouldThrowTeamNotFoundException()
    {
        // Arrange
        var nonExistentTeam = new Team
        {
            Id = 999,
            TeamName = "Ghost Team",
            BusinessUnit = "Nowhere",
            PTL = "Nobody",
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<TeamNotFoundException>(async () =>
            await _repository.UpdateTeamAsync(nonExistentTeam)
        );
        Assert.That(ex.Message, Does.Contain(nonExistentTeam.Id.ToString()));
    }

    [Test]
    public async Task GetTeamAsync_ExistingTeam_ShouldReturnCorrectTeam()
    {
        // Arrange
        var expectedTeam = new Team
        {
            Id = 400,
            TeamName = "Team Zeta",
            BusinessUnit = "Core Services",
            PTL = "Lead Zeta",
        };
        _context.Teams.Add(expectedTeam);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetTeamAsync(expectedTeam.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(expectedTeam.Id));
            Assert.That(result.TeamName, Is.EqualTo(expectedTeam.TeamName));
            Assert.That(result.BusinessUnit, Is.EqualTo(expectedTeam.BusinessUnit));
            Assert.That(result.PTL, Is.EqualTo(expectedTeam.PTL));
        });
    }
}
