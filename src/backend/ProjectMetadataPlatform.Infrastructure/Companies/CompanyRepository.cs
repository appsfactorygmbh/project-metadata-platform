using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Companies;
using ProjectMetadataPlatform.Domain.Errors.CompanyExceptions;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Companies;

/// <summary>
/// The repository for companies that handles the data access.
/// </summary>
public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
{
    private readonly ProjectMetadataPlatformDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompanyRepository" /> class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing company data.</param>
    public CompanyRepository(ProjectMetadataPlatformDbContext dbContext)
        : base(dbContext)
    {
        _context = dbContext;
    }

    /// <inheritdoc/>
    public async Task<IQueryable<Company>> GetCompaniesAsync()
    {
        return _context.Companies.AsNoTracking();
    }

    /// <inheritdoc/>
    public async Task<Company> GetCompanyAsync(int id)
    {
        return await _context.Companies.FirstOrDefaultAsync(company => company.Id == id)
            ?? throw new CompanyNotFoundException(id);
    }

    /// <inheritdoc/>
    public async Task<bool> CheckIfCompanyExistsAsync(int id)
    {
        return await _context.Companies.AnyAsync(company => company.Id == id);
    }

    /// <inheritdoc/>
    public async Task<bool> CheckIfCompanyNameExistsAsync(string companyName)
    {
        return await _context.Companies.AnyAsync(company => company.CompanyName == companyName);
    }

    /// <inheritdoc/>
    public async Task<string> RetrieveNameForIdAsync(int id)
    {
        return (
            (await _context.Companies.FirstOrDefaultAsync(company => company.Id == id))
            ?? throw new CompanyNotFoundException(id)
        ).CompanyName;
    }

    /// <inheritdoc/>
    public async Task<Company> GetCompanyByNameAsync(string companyName)
    {
        return await _context.Companies.FirstOrDefaultAsync(c => c.CompanyName == companyName)
            ?? throw new CompanyNotFoundException(companyName);
    }

    /// <inheritdoc/>
    public async Task AddCompanyAsync(Company company)
    {
        if (!await GetIf(c => c.Id == company.Id).AnyAsync())
        {
            _ = _context.Companies.Add(company);
        }
    }

    /// <inheritdoc/>
    public async Task<Company> UpdateCompanyAsync(Company company)
    {
        if (!await CheckIfCompanyExistsAsync(company.Id))
        {
            throw new CompanyNotFoundException(company.Id);
        }
        _ = _context.Companies.Update(company);
        return company;
    }

    /// <inheritdoc/>
    public async Task<Company> DeleteCompanyAsync(Company company)
    {
        _ = _context.Companies.Remove(company);
        return await Task.FromResult(company);
    }

    /// <inheritdoc/>
    public async Task<Company> GetCompanyWithProjectsAsync(int id)
    {
        return await _context
                .Companies.Include(company => company.Projects)
                .FirstOrDefaultAsync(company => company.Id == id)
            ?? throw new CompanyNotFoundException(id);
    }
}
