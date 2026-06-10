using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProjectMetadataPlatform.Domain.Departments;
using ProjectMetadataPlatform.Domain.Errors.DepartmentExceptions;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Infrastructure.Departments;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

[TestFixture]
public class DepartmentsRepositoryTests : TestsWithDatabase
{
    [SetUp]
    public void Setup()
    {
        _context = DbContext();
        _repository = new DepartmentRepository(_context);
        ClearData(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    private ProjectMetadataPlatformDbContext _context;
    private DepartmentRepository _repository;

    [Test]
    public async Task GetDepartmentsAsync_ShouldReturnAllDepartments()
    {
        // Arrange
        var department = new Department { Id = 1, DepartmentName = "Test_1" };

        var department2 = new Department { Id = 2, DepartmentName = "Test_2" };

        _context.Departments.RemoveRange(_context.Departments);
        _ = _context.Departments.Add(department);
        _ = _context.Departments.Add(department2);
        _ = await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetDepartmentsAsync()).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        var departmentRes = result.First();
        var departmentRes2 = result.Last();
        Assert.Multiple(() =>
        {
            Assert.That(departmentRes.Id, Is.EqualTo(1));
            Assert.That(departmentRes.DepartmentName, Is.EqualTo("Test_1"));

            Assert.That(departmentRes2.Id, Is.EqualTo(2));
            Assert.That(departmentRes2.DepartmentName, Is.EqualTo("Test_2"));
        });
    }

    [Test]
    public async Task DeleteProjectAsync_ShouldDeleteProject()
    {
        // Arrange
        var department = new Department { Id = 1, DepartmentName = "Test_1" };

        _context.Departments.RemoveRange(_context.Departments);
        _ = _context.Departments.Add(department);
        _ = await _context.SaveChangesAsync();

        // Act
        var deletedDepartment = await _repository.DeleteDepartmentAsync(department);
        _ = await _context.SaveChangesAsync();

        // Assert
        var remainingDepartments = _context.Departments.ToList();

        Assert.Multiple(() =>
        {
            Assert.That(remainingDepartments, Is.Empty);
            Assert.That(deletedDepartment, Is.EqualTo(department));
        });
    }

    [Test]
    public async Task GetDepartmentAsync_ShouldThrowDepartmentNotFoundExceptionIfNotPresent()
    {
        // Arrange
        _context.Departments.RemoveRange(_context.Departments);
        _ = await _context.SaveChangesAsync();

        // Act + Assert
        var ex = Assert.ThrowsAsync<DepartmentNotFoundException>(async () =>
            await _repository.GetDepartmentAsync(1)
        );
        Assert.That(ex.Message, Does.Contain("1"));
    }

    [Test]
    public async Task AddDepartmentAsync_NewDepartment_ShouldAddDepartmentToDatabase()
    {
        // Arrange
        var newDepartment = new Department { Id = 100, DepartmentName = "New Department Alpha" };

        // Act
        _context.Departments.RemoveRange(_context.Departments);
        await _repository.AddDepartmentAsync(newDepartment);
        _ = await _context.SaveChangesAsync();

        // Assert
        var addedDepartment = await _context.Departments.FindAsync(newDepartment.Id);
        Assert.That(addedDepartment, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(addedDepartment.DepartmentName, Is.EqualTo(newDepartment.DepartmentName));
        });
        Assert.That(await _context.Departments.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task AddDepartmentAsync_DepartmentWithExistingId_ShouldNotAddOrUpdateDepartment()
    {
        // Arrange
        var initialDepartment = new Department { Id = 101, DepartmentName = "Original Gamma" };
        _context.Departments.RemoveRange(_context.Departments);
        _ = _context.Departments.Add(initialDepartment);
        _ = await _context.SaveChangesAsync();

        var departmentWithSameId = new Department
        {
            Id = 101,
            DepartmentName = "Updated Gamma Attempt",
        };

        // Act
        await _repository.AddDepartmentAsync(departmentWithSameId);
        _ = await _context.SaveChangesAsync();

        // Assert
        var departmentInDb = await _context.Departments.FindAsync(initialDepartment.Id);
        Assert.That(departmentInDb, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(
                departmentInDb.DepartmentName,
                Is.EqualTo(initialDepartment.DepartmentName)
            );
        });
        Assert.That(await _context.Departments.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task CheckIfDepartmentNameExistsAsync_NameExists_ShouldReturnTrue()
    {
        // Arrange
        var existingDepartmentName = "Unique Existent Department";
        _ = _context.Departments.Add(
            new Department { Id = 200, DepartmentName = existingDepartmentName }
        );
        _ = await _context.SaveChangesAsync();

        // Act
        var result = await _repository.CheckIfDepartmentNameExistsAsync(existingDepartmentName);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckIfDepartmentNameExistsAsync_NameDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentDepartmentName = "Definitely Not Here Department";
        _ = _context.Departments.Add(
            new Department { Id = 201, DepartmentName = "Some Other Department" }
        );
        _ = await _context.SaveChangesAsync();

        // Act
        var result = await _repository.CheckIfDepartmentNameExistsAsync(nonExistentDepartmentName);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CheckIfDepartmentNameExistsAsync_EmptyDatabase_ShouldReturnFalse()
    {
        // Arrange

        // Act
        var result = await _repository.CheckIfDepartmentNameExistsAsync("Any Name");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task UpdateDepartmentAsync_ExistingDepartment_ShouldUpdateDepartmentProperties()
    {
        // Arrange
        var initialDepartment = new Department { Id = 300, DepartmentName = "Department Epsilon" };
        _ = _context.Departments.Add(initialDepartment);
        _ = await _context.SaveChangesAsync();
        _context.Entry(initialDepartment).State = EntityState.Detached;

        var updatedDepartmentData = new Department
        {
            Id = 300,
            DepartmentName = "Department Epsilon Updated",
        };

        // Act
        var result = await _repository.UpdateDepartmentAsync(updatedDepartmentData);
        _ = await _context.SaveChangesAsync();

        // Assert
        var departmentFromDb = await _context.Departments.FindAsync(initialDepartment.Id);
        Assert.That(departmentFromDb, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(
                departmentFromDb.DepartmentName,
                Is.EqualTo(updatedDepartmentData.DepartmentName)
            );
            Assert.That(result.DepartmentName, Is.EqualTo(updatedDepartmentData.DepartmentName));
        });
    }

    [Test]
    public void UpdateDepartmentAsync_NonExistingDepartment_ShouldThrowDepartmentNotFoundException()
    {
        // Arrange
        var nonExistentDepartment = new Department
        {
            Id = 999,
            DepartmentName = "Ghost Department",
        };

        // Act & Assert
        var ex = Assert.ThrowsAsync<DepartmentNotFoundException>(async () =>
            await _repository.UpdateDepartmentAsync(nonExistentDepartment)
        );
        Assert.That(ex.Message, Does.Contain(nonExistentDepartment.Id.ToString()));
    }

    [Test]
    public async Task GetDepartmentAsync_ExistingDepartment_ShouldReturnCorrectDepartment()
    {
        // Arrange
        var expectedDepartment = new Department { Id = 400, DepartmentName = "Department Zeta" };
        _ = _context.Departments.Add(expectedDepartment);
        _ = await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetDepartmentAsync(expectedDepartment.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(expectedDepartment.Id));
            Assert.That(result.DepartmentName, Is.EqualTo(expectedDepartment.DepartmentName));
        });
    }
}
