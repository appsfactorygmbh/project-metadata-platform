using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProjectMetadataPlatform.Domain.Companies;
using ProjectMetadataPlatform.Domain.Errors.CompanyExceptions;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Infrastructure.Companies;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

[TestFixture]
public class CompaniesRepositoryTests : TestsWithDatabase
{
    [SetUp]
    public void Setup()
    {
        _context = DbContext();
        _repository = new CompanyRepository(_context);
        ClearData(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    private ProjectMetadataPlatformDbContext _context;
    private CompanyRepository _repository;

    [Test]
    public async Task GetCompaniesAsync_ShouldReturnAllCompanies()
    {
        // Arrange
        var company = new Company { Id = 1, CompanyName = "Test_1" };

        var company2 = new Company { Id = 2, CompanyName = "Test_2" };

        _context.Companies.RemoveRange(_context.Companies);
        _context.Companies.Add(company);
        _context.Companies.Add(company2);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetCompaniesAsync()).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        var companyRes = result.First();
        var companyRes2 = result.Last();
        Assert.Multiple(() =>
        {
            Assert.That(companyRes.Id, Is.EqualTo(1));
            Assert.That(companyRes.CompanyName, Is.EqualTo("Test_1"));

            Assert.That(companyRes2.Id, Is.EqualTo(2));
            Assert.That(companyRes2.CompanyName, Is.EqualTo("Test_2"));
        });
    }

    [Test]
    public async Task DeleteProjectAsync_ShouldDeleteProject()
    {
        // Arrange
        var company = new Company { Id = 1, CompanyName = "Test_1" };

        _context.Companies.RemoveRange(_context.Companies);
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        // Act
        var deletedCompany = await _repository.DeleteCompanyAsync(company);
        await _context.SaveChangesAsync();

        // Assert
        var remainingCompanies = _context.Companies.ToList();

        Assert.Multiple(() =>
        {
            Assert.That(remainingCompanies, Is.Empty);
            Assert.That(deletedCompany, Is.EqualTo(company));
        });
    }

    [Test]
    public async Task GetCompanyWithProjectsAsync_ShouldReturnCompanyWithAssociatedProjects()
    {
        // Arrange
        var company = new Company { Id = 1, CompanyName = "Test_1" };

        var project = new Project
        {
            Id = 1,
            ProjectName = "Nieselegen",
            Slug = "nieselregen",
            ClientName = "Nasa",
            CompanyId = 1,
        };

        _context.Companies.RemoveRange(_context.Companies);
        _context.Companies.Add(company);

        _context.Projects.RemoveRange(_context.Projects);
        _context.Projects.Add(project);

        await _context.SaveChangesAsync();

        // Act
        var companyWithProjects = await _repository.GetCompanyWithProjectsAsync(1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(companyWithProjects.CompanyName, Is.EqualTo("Test_1"));
            Assert.That(companyWithProjects.Id, Is.EqualTo(1));
            Assert.That(companyWithProjects.Projects, Is.Not.Null);
            Assert.That(companyWithProjects.Projects, Has.Count.EqualTo(1));
            var linkedProject = companyWithProjects.Projects!.First();

            Assert.That(linkedProject.Id, Is.EqualTo(1));
            Assert.That(linkedProject.ProjectName, Is.EqualTo("Nieselegen"));
            Assert.That(linkedProject.Slug, Is.EqualTo("nieselregen"));
            Assert.That(linkedProject.ClientName, Is.EqualTo("Nasa"));
        });
    }

    [Test]
    public async Task GetCompanyAsync_ShouldThrowCompanyNotFoundExceptionIfNotPresent()
    {
        // Arrange
        _context.Companies.RemoveRange(_context.Companies);
        await _context.SaveChangesAsync();

        // Act + Assert
        var ex = Assert.ThrowsAsync<CompanyNotFoundException>(async () =>
            await _repository.GetCompanyAsync(1)
        );
        Assert.That(ex.Message, Does.Contain("1"));
    }

    [Test]
    public async Task RetrieveNameForIdAsync_ShouldReturnCorrectName()
    {
        // Arrange
        var company = new Company { Id = 1, CompanyName = "Test_1" };
        _context.Companies.RemoveRange(_context.Companies);
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        // Act
        var companyName = await _repository.RetrieveNameForIdAsync(1);

        // Assert
        Assert.That(companyName, Is.EqualTo("Test_1"));
    }

    [Test]
    public async Task AddCompanyAsync_NewCompany_ShouldAddCompanyToDatabase()
    {
        // Arrange
        var newCompany = new Company { Id = 100, CompanyName = "New Company Alpha" };

        // Act
        _context.Companies.RemoveRange(_context.Companies);
        await _repository.AddCompanyAsync(newCompany);
        await _context.SaveChangesAsync();

        // Assert
        var addedCompany = await _context.Companies.FindAsync(newCompany.Id);
        Assert.That(addedCompany, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(addedCompany.CompanyName, Is.EqualTo(newCompany.CompanyName));
        });
        Assert.That(await _context.Companies.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task AddCompanyAsync_CompanyWithExistingId_ShouldNotAddOrUpdateCompany()
    {
        // Arrange
        var initialCompany = new Company { Id = 101, CompanyName = "Original Gamma" };
        _context.Companies.RemoveRange(_context.Companies);
        _context.Companies.Add(initialCompany);
        await _context.SaveChangesAsync();

        var companyWithSameId = new Company { Id = 101, CompanyName = "Updated Gamma Attempt" };

        // Act
        await _repository.AddCompanyAsync(companyWithSameId);
        await _context.SaveChangesAsync();

        // Assert
        var companyInDb = await _context.Companies.FindAsync(initialCompany.Id);
        Assert.That(companyInDb, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(companyInDb.CompanyName, Is.EqualTo(initialCompany.CompanyName));
        });
        Assert.That(await _context.Companies.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task CheckIfCompanyNameExistsAsync_NameExists_ShouldReturnTrue()
    {
        // Arrange
        var existingCompanyName = "Unique Existent Company";
        _context.Companies.Add(new Company { Id = 200, CompanyName = existingCompanyName });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.CheckIfCompanyNameExistsAsync(existingCompanyName);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckIfCompanyNameExistsAsync_NameDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentCompanyName = "Definitely Not Here Company";
        _context.Companies.Add(new Company { Id = 201, CompanyName = "Some Other Company" });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.CheckIfCompanyNameExistsAsync(nonExistentCompanyName);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CheckIfCompanyNameExistsAsync_EmptyDatabase_ShouldReturnFalse()
    {
        // Arrange

        // Act
        var result = await _repository.CheckIfCompanyNameExistsAsync("Any Name");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task UpdateCompanyAsync_ExistingCompany_ShouldUpdateCompanyProperties()
    {
        // Arrange
        var initialCompany = new Company { Id = 300, CompanyName = "Company Epsilon" };
        _context.Companies.Add(initialCompany);
        await _context.SaveChangesAsync();
        _context.Entry(initialCompany).State = EntityState.Detached;

        var updatedCompanyData = new Company { Id = 300, CompanyName = "Company Epsilon Updated" };

        // Act
        var result = await _repository.UpdateCompanyAsync(updatedCompanyData);
        await _context.SaveChangesAsync();

        // Assert
        var companyFromDb = await _context.Companies.FindAsync(initialCompany.Id);
        Assert.That(companyFromDb, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(companyFromDb.CompanyName, Is.EqualTo(updatedCompanyData.CompanyName));
            Assert.That(result.CompanyName, Is.EqualTo(updatedCompanyData.CompanyName));
        });
    }

    [Test]
    public void UpdateCompanyAsync_NonExistingCompany_ShouldThrowCompanyNotFoundException()
    {
        // Arrange
        var nonExistentCompany = new Company { Id = 999, CompanyName = "Ghost Company" };

        // Act & Assert
        var ex = Assert.ThrowsAsync<CompanyNotFoundException>(async () =>
            await _repository.UpdateCompanyAsync(nonExistentCompany)
        );
        Assert.That(ex.Message, Does.Contain(nonExistentCompany.Id.ToString()));
    }

    [Test]
    public async Task GetCompanyAsync_ExistingCompany_ShouldReturnCorrectCompany()
    {
        // Arrange
        var expectedCompany = new Company { Id = 400, CompanyName = "Company Zeta" };
        _context.Companies.Add(expectedCompany);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetCompanyAsync(expectedCompany.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(expectedCompany.Id));
            Assert.That(result.CompanyName, Is.EqualTo(expectedCompany.CompanyName));
        });
    }
}
