using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;
using ProjectMetadataPlatform.Domain.Teams;
using ProjectMetadataPlatform.Infrastructure.BusinessUnits;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

[TestFixture]
public class BusinessUnitsRepositoryTests : TestsWithDatabase
{
    [SetUp]
    public void Setup()
    {
        _context = DbContext();
        _repository = new BusinessUnitRepository(_context);
        ClearData(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    private ProjectMetadataPlatformDbContext _context;
    private BusinessUnitRepository _repository;

    [Test]
    public async Task GetBusinessUnitsAsync_ShouldReturnAllBusinessUnits()
    {
        // Arrange
        var businessUnit = new BusinessUnit { Id = 1, BusinessUnitName = "Test_1" };

        var businessUnit2 = new BusinessUnit { Id = 2, BusinessUnitName = "Test_2" };

        _context.BusinessUnits.RemoveRange(_context.BusinessUnits);
        _ = _context.BusinessUnits.Add(businessUnit);
        _ = _context.BusinessUnits.Add(businessUnit2);
        _ = await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetBusinessUnitsAsync()).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        var businessUnitRes = result.First();
        var businessUnitRes2 = result.Last();
        Assert.Multiple(() =>
        {
            Assert.That(businessUnitRes.Id, Is.EqualTo(1));
            Assert.That(businessUnitRes.BusinessUnitName, Is.EqualTo("Test_1"));

            Assert.That(businessUnitRes2.Id, Is.EqualTo(2));
            Assert.That(businessUnitRes2.BusinessUnitName, Is.EqualTo("Test_2"));
        });
    }

    [Test]
    public async Task DeleteTeamAsync_ShouldDeleteTeam()
    {
        // Arrange
        var businessUnit = new BusinessUnit { Id = 1, BusinessUnitName = "Test_1" };

        _context.BusinessUnits.RemoveRange(_context.BusinessUnits);
        _ = _context.BusinessUnits.Add(businessUnit);
        _ = await _context.SaveChangesAsync();

        // Act
        var deletedBusinessUnit = await _repository.DeleteBusinessUnitAsync(businessUnit);
        _ = await _context.SaveChangesAsync();

        // Assert
        var remainingBusinessUnits = _context.BusinessUnits.ToList();

        Assert.Multiple(() =>
        {
            Assert.That(remainingBusinessUnits, Is.Empty);
            Assert.That(deletedBusinessUnit, Is.EqualTo(businessUnit));
        });
    }

    [Test]
    public async Task GetBusinessUnitWithTeamsAsync_ShouldReturnBusinessUnitWithAssociatedTeams()
    {
        // Arrange
        var businessUnit = new BusinessUnit { Id = 1, BusinessUnitName = "Test_1" };

        var team = new Team
        {
            Id = 1,
            TeamName = "Nieselegen",
            BusinessUnitId = 1,
        };

        _context.BusinessUnits.RemoveRange(_context.BusinessUnits);
        _ = _context.BusinessUnits.Add(businessUnit);

        _context.Teams.RemoveRange(_context.Teams);
        _ = _context.Teams.Add(team);

        _ = await _context.SaveChangesAsync();

        // Act
        var businessUnitWithTeams = await _repository.GetBusinessUnitWithTeamsAsync(1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(businessUnitWithTeams.BusinessUnitName, Is.EqualTo("Test_1"));
            Assert.That(businessUnitWithTeams.Id, Is.EqualTo(1));
            Assert.That(businessUnitWithTeams.Teams, Is.Not.Null);
            Assert.That(businessUnitWithTeams.Teams, Has.Count.EqualTo(1));
            var linkedTeam = businessUnitWithTeams.Teams!.First();

            Assert.That(linkedTeam.Id, Is.EqualTo(1));
            Assert.That(linkedTeam.TeamName, Is.EqualTo("Nieselegen"));
        });
    }

    [Test]
    public async Task GetBusinessUnitAsync_ShouldThrowBusinessUnitNotFoundExceptionIfNotPresent()
    {
        // Arrange
        _context.BusinessUnits.RemoveRange(_context.BusinessUnits);
        _ = await _context.SaveChangesAsync();

        // Act + Assert
        var ex = Assert.ThrowsAsync<BusinessUnitNotFoundException>(async () =>
            await _repository.GetBusinessUnitAsync(1)
        );
        Assert.That(ex.Message, Does.Contain("1"));
    }

    [Test]
    public async Task RetrieveNameForIdAsync_ShouldReturnCorrectName()
    {
        // Arrange
        var businessUnit = new BusinessUnit { Id = 1, BusinessUnitName = "Test_1" };
        _context.BusinessUnits.RemoveRange(_context.BusinessUnits);
        _ = _context.BusinessUnits.Add(businessUnit);
        _ = await _context.SaveChangesAsync();

        // Act
        var businessUnitName = await _repository.RetrieveNameForIdAsync(1);

        // Assert
        Assert.That(businessUnitName, Is.EqualTo("Test_1"));
    }

    [Test]
    public async Task AddBusinessUnitAsync_NewBusinessUnit_ShouldAddBusinessUnitToDatabase()
    {
        // Arrange
        var newBusinessUnit = new BusinessUnit
        {
            Id = 100,
            BusinessUnitName = "New BusinessUnit Alpha",
        };

        // Act
        _context.BusinessUnits.RemoveRange(_context.BusinessUnits);
        await _repository.AddBusinessUnitAsync(newBusinessUnit);
        _ = await _context.SaveChangesAsync();

        // Assert
        var addedBusinessUnit = await _context.BusinessUnits.FindAsync(newBusinessUnit.Id);
        Assert.That(addedBusinessUnit, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(
                addedBusinessUnit.BusinessUnitName,
                Is.EqualTo(newBusinessUnit.BusinessUnitName)
            );
        });
        Assert.That(await _context.BusinessUnits.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task AddBusinessUnitAsync_BusinessUnitWithExistingId_ShouldNotAddOrUpdateBusinessUnit()
    {
        // Arrange
        var initialBusinessUnit = new BusinessUnit
        {
            Id = 101,
            BusinessUnitName = "Original Gamma",
        };
        _context.BusinessUnits.RemoveRange(_context.BusinessUnits);
        _ = _context.BusinessUnits.Add(initialBusinessUnit);
        _ = await _context.SaveChangesAsync();

        var businessUnitWithSameId = new BusinessUnit
        {
            Id = 101,
            BusinessUnitName = "Updated Gamma Attempt",
        };

        // Act
        await _repository.AddBusinessUnitAsync(businessUnitWithSameId);
        _ = await _context.SaveChangesAsync();

        // Assert
        var businessUnitInDb = await _context.BusinessUnits.FindAsync(initialBusinessUnit.Id);
        Assert.That(businessUnitInDb, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(
                businessUnitInDb.BusinessUnitName,
                Is.EqualTo(initialBusinessUnit.BusinessUnitName)
            );
        });
        Assert.That(await _context.BusinessUnits.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task CheckIfBusinessUnitNameExistsAsync_NameExists_ShouldReturnTrue()
    {
        // Arrange
        var existingBusinessUnitName = "Unique Existent BusinessUnit";
        _ = _context.BusinessUnits.Add(
            new BusinessUnit { Id = 200, BusinessUnitName = existingBusinessUnitName }
        );
        _ = await _context.SaveChangesAsync();

        // Act
        var result = await _repository.CheckIfBusinessUnitNameExistsAsync(existingBusinessUnitName);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckIfBusinessUnitNameExistsAsync_NameDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentBusinessUnitName = "Definitely Not Here BusinessUnit";
        _ = _context.BusinessUnits.Add(
            new BusinessUnit { Id = 201, BusinessUnitName = "Some Other BusinessUnit" }
        );
        _ = await _context.SaveChangesAsync();

        // Act
        var result = await _repository.CheckIfBusinessUnitNameExistsAsync(
            nonExistentBusinessUnitName
        );

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CheckIfBusinessUnitNameExistsAsync_EmptyDatabase_ShouldReturnFalse()
    {
        // Arrange

        // Act
        var result = await _repository.CheckIfBusinessUnitNameExistsAsync("Any Name");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task UpdateBusinessUnitAsync_ExistingBusinessUnit_ShouldUpdateBusinessUnitProperties()
    {
        // Arrange
        var initialBusinessUnit = new BusinessUnit
        {
            Id = 300,
            BusinessUnitName = "BusinessUnit Epsilon",
        };
        _ = _context.BusinessUnits.Add(initialBusinessUnit);
        _ = await _context.SaveChangesAsync();
        _context.Entry(initialBusinessUnit).State = EntityState.Detached;

        var updatedBusinessUnitData = new BusinessUnit
        {
            Id = 300,
            BusinessUnitName = "BusinessUnit Epsilon Updated",
        };

        // Act
        var result = await _repository.UpdateBusinessUnitAsync(updatedBusinessUnitData);
        _ = await _context.SaveChangesAsync();

        // Assert
        var businessUnitFromDb = await _context.BusinessUnits.FindAsync(initialBusinessUnit.Id);
        Assert.That(businessUnitFromDb, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(
                businessUnitFromDb.BusinessUnitName,
                Is.EqualTo(updatedBusinessUnitData.BusinessUnitName)
            );
            Assert.That(
                result.BusinessUnitName,
                Is.EqualTo(updatedBusinessUnitData.BusinessUnitName)
            );
        });
    }

    [Test]
    public void UpdateBusinessUnitAsync_NonExistingBusinessUnit_ShouldThrowBusinessUnitNotFoundException()
    {
        // Arrange
        var nonExistentBusinessUnit = new BusinessUnit
        {
            Id = 999,
            BusinessUnitName = "Ghost BusinessUnit",
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<BusinessUnitNotFoundException>(async () =>
            await _repository.UpdateBusinessUnitAsync(nonExistentBusinessUnit)
        );
        Assert.That(ex.Message, Does.Contain(nonExistentBusinessUnit.Id.ToString()));
    }

    [Test]
    public async Task GetBusinessUnitAsync_ExistingBusinessUnit_ShouldReturnCorrectBusinessUnit()
    {
        // Arrange
        var expectedBusinessUnit = new BusinessUnit
        {
            Id = 400,
            BusinessUnitName = "BusinessUnit Zeta",
        };
        _ = _context.BusinessUnits.Add(expectedBusinessUnit);
        _ = await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetBusinessUnitAsync(expectedBusinessUnit.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(expectedBusinessUnit.Id));
            Assert.That(result.BusinessUnitName, Is.EqualTo(expectedBusinessUnit.BusinessUnitName));
        });
    }
}
