using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProjectMetadataPlatform.Domain.Errors.OfficeLocationExceptions;
using ProjectMetadataPlatform.Domain.OfficeLocations;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Infrastructure.OfficeLocations;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

[TestFixture]
public class OfficeLocationsRepositoryTests : TestsWithDatabase
{
    [SetUp]
    public void Setup()
    {
        _context = DbContext();
        _repository = new OfficeLocationRepository(_context);
        ClearData(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    private ProjectMetadataPlatformDbContext _context;
    private OfficeLocationRepository _repository;

    [Test]
    public async Task GetOfficeLocationsAsync_ShouldReturnAllOfficeLocations()
    {
        // Arrange
        var officeLocation = new OfficeLocation { Id = 1, OfficeLocationName = "Test_1" };

        var officeLocation2 = new OfficeLocation { Id = 2, OfficeLocationName = "Test_2" };

        _context.OfficeLocations.RemoveRange(_context.OfficeLocations);
        _ = _context.OfficeLocations.Add(officeLocation);
        _ = _context.OfficeLocations.Add(officeLocation2);
        _ = await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetOfficeLocationsAsync()).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        var officeLocationRes = result.First();
        var officeLocationRes2 = result.Last();
        Assert.Multiple(() =>
        {
            Assert.That(officeLocationRes.Id, Is.EqualTo(1));
            Assert.That(officeLocationRes.OfficeLocationName, Is.EqualTo("Test_1"));

            Assert.That(officeLocationRes2.Id, Is.EqualTo(2));
            Assert.That(officeLocationRes2.OfficeLocationName, Is.EqualTo("Test_2"));
        });
    }

    [Test]
    public async Task DeleteProjectAsync_ShouldDeleteProject()
    {
        // Arrange
        var officeLocation = new OfficeLocation { Id = 1, OfficeLocationName = "Test_1" };

        _context.OfficeLocations.RemoveRange(_context.OfficeLocations);
        _ = _context.OfficeLocations.Add(officeLocation);
        _ = await _context.SaveChangesAsync();

        // Act
        var deletedOfficeLocation = await _repository.DeleteOfficeLocationAsync(officeLocation);
        _ = await _context.SaveChangesAsync();

        // Assert
        var remainingOfficeLocations = _context.OfficeLocations.ToList();

        Assert.Multiple(() =>
        {
            Assert.That(remainingOfficeLocations, Is.Empty);
            Assert.That(deletedOfficeLocation, Is.EqualTo(officeLocation));
        });
    }

    [Test]
    public async Task GetOfficeLocationAsync_ShouldThrowOfficeLocationNotFoundExceptionIfNotPresent()
    {
        // Arrange
        _context.OfficeLocations.RemoveRange(_context.OfficeLocations);
        _ = await _context.SaveChangesAsync();

        // Act + Assert
        var ex = Assert.ThrowsAsync<OfficeLocationNotFoundException>(async () =>
            await _repository.GetOfficeLocationAsync(1)
        );
        Assert.That(ex.Message, Does.Contain("1"));
    }

    [Test]
    public async Task AddOfficeLocationAsync_NewOfficeLocation_ShouldAddOfficeLocationToDatabase()
    {
        // Arrange
        var newOfficeLocation = new OfficeLocation
        {
            Id = 100,
            OfficeLocationName = "New OfficeLocation Alpha",
        };

        // Act
        _context.OfficeLocations.RemoveRange(_context.OfficeLocations);
        await _repository.AddOfficeLocationAsync(newOfficeLocation);
        _ = await _context.SaveChangesAsync();

        // Assert
        var addedOfficeLocation = await _context.OfficeLocations.FindAsync(newOfficeLocation.Id);
        Assert.That(addedOfficeLocation, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(
                addedOfficeLocation.OfficeLocationName,
                Is.EqualTo(newOfficeLocation.OfficeLocationName)
            );
        });
        Assert.That(await _context.OfficeLocations.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task AddOfficeLocationAsync_OfficeLocationWithExistingId_ShouldNotAddOrUpdateOfficeLocation()
    {
        // Arrange
        var initialOfficeLocation = new OfficeLocation
        {
            Id = 101,
            OfficeLocationName = "Original Gamma",
        };
        _context.OfficeLocations.RemoveRange(_context.OfficeLocations);
        _ = _context.OfficeLocations.Add(initialOfficeLocation);
        _ = await _context.SaveChangesAsync();

        var officeLocationWithSameId = new OfficeLocation
        {
            Id = 101,
            OfficeLocationName = "Updated Gamma Attempt",
        };

        // Act
        await _repository.AddOfficeLocationAsync(officeLocationWithSameId);
        _ = await _context.SaveChangesAsync();

        // Assert
        var officeLocationInDb = await _context.OfficeLocations.FindAsync(initialOfficeLocation.Id);
        Assert.That(officeLocationInDb, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(
                officeLocationInDb.OfficeLocationName,
                Is.EqualTo(initialOfficeLocation.OfficeLocationName)
            );
        });
        Assert.That(await _context.OfficeLocations.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task CheckIfOfficeLocationNameExistsAsync_NameExists_ShouldReturnTrue()
    {
        // Arrange
        var existingOfficeLocationName = "Unique Existent OfficeLocation";
        _ = _context.OfficeLocations.Add(
            new OfficeLocation { Id = 200, OfficeLocationName = existingOfficeLocationName }
        );
        _ = await _context.SaveChangesAsync();

        // Act
        var result = await _repository.CheckIfOfficeLocationNameExistsAsync(
            existingOfficeLocationName
        );

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckIfOfficeLocationNameExistsAsync_NameDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentOfficeLocationName = "Definitely Not Here OfficeLocation";
        _ = _context.OfficeLocations.Add(
            new OfficeLocation { Id = 201, OfficeLocationName = "Some Other OfficeLocation" }
        );
        _ = await _context.SaveChangesAsync();

        // Act
        var result = await _repository.CheckIfOfficeLocationNameExistsAsync(
            nonExistentOfficeLocationName
        );

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CheckIfOfficeLocationNameExistsAsync_EmptyDatabase_ShouldReturnFalse()
    {
        // Arrange

        // Act
        var result = await _repository.CheckIfOfficeLocationNameExistsAsync("Any Name");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task UpdateOfficeLocationAsync_ExistingOfficeLocation_ShouldUpdateOfficeLocationProperties()
    {
        // Arrange
        var initialOfficeLocation = new OfficeLocation
        {
            Id = 300,
            OfficeLocationName = "OfficeLocation Epsilon",
        };
        _ = _context.OfficeLocations.Add(initialOfficeLocation);
        _ = await _context.SaveChangesAsync();
        _context.Entry(initialOfficeLocation).State = EntityState.Detached;

        var updatedOfficeLocationData = new OfficeLocation
        {
            Id = 300,
            OfficeLocationName = "OfficeLocation Epsilon Updated",
        };

        // Act
        var result = await _repository.UpdateOfficeLocationAsync(updatedOfficeLocationData);
        _ = await _context.SaveChangesAsync();

        // Assert
        var officeLocationFromDb = await _context.OfficeLocations.FindAsync(
            initialOfficeLocation.Id
        );
        Assert.That(officeLocationFromDb, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(
                officeLocationFromDb.OfficeLocationName,
                Is.EqualTo(updatedOfficeLocationData.OfficeLocationName)
            );
            Assert.That(
                result.OfficeLocationName,
                Is.EqualTo(updatedOfficeLocationData.OfficeLocationName)
            );
        });
    }

    [Test]
    public void UpdateOfficeLocationAsync_NonExistingOfficeLocation_ShouldThrowOfficeLocationNotFoundException()
    {
        // Arrange
        var nonExistentOfficeLocation = new OfficeLocation
        {
            Id = 999,
            OfficeLocationName = "Ghost OfficeLocation",
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<OfficeLocationNotFoundException>(async () =>
            await _repository.UpdateOfficeLocationAsync(nonExistentOfficeLocation)
        );
        Assert.That(ex.Message, Does.Contain(nonExistentOfficeLocation.Id.ToString()));
    }

    [Test]
    public async Task GetOfficeLocationAsync_ExistingOfficeLocation_ShouldReturnCorrectOfficeLocation()
    {
        // Arrange
        var expectedOfficeLocation = new OfficeLocation
        {
            Id = 400,
            OfficeLocationName = "OfficeLocation Zeta",
        };
        _ = _context.OfficeLocations.Add(expectedOfficeLocation);
        _ = await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetOfficeLocationAsync(expectedOfficeLocation.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(expectedOfficeLocation.Id));
            Assert.That(
                result.OfficeLocationName,
                Is.EqualTo(expectedOfficeLocation.OfficeLocationName)
            );
        });
    }
}
