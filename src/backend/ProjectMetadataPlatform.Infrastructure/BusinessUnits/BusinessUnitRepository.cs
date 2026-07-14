using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.BusinessUnits;

/// <summary>
/// The repository for business Units that handles the data access.
/// </summary>
public class BusinessUnitRepository : RepositoryBase<BusinessUnit>, IBusinessUnitRepository
{
    private readonly ProjectMetadataPlatformDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessUnitRepository" /> class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing bu data.</param>
    public BusinessUnitRepository(ProjectMetadataPlatformDbContext dbContext)
        : base(dbContext)
    {
        _context = dbContext;
    }

    /// <inheritdoc/>
    public async Task<IQueryable<BusinessUnit>> GetBusinessUnitsAsync()
    {
        return _context.BusinessUnits.AsNoTracking();
    }

    /// <inheritdoc/>
    public async Task<BusinessUnit> GetBusinessUnitAsync(int id)
    {
        return await _context.BusinessUnits.FirstOrDefaultAsync(businessUnit =>
                businessUnit.Id == id
            ) ?? throw new BusinessUnitNotFoundException(id);
    }

    /// <inheritdoc/>
    public async Task<bool> CheckIfBusinessUnitExistsAsync(int id)
    {
        return await _context.BusinessUnits.AnyAsync(bu => bu.Id == id);
    }

    /// <inheritdoc/>
    public async Task<bool> CheckIfBusinessUnitNameExistsAsync(string buName)
    {
        return await _context.BusinessUnits.AnyAsync(bu => bu.BusinessUnitName == buName);
    }

    /// <inheritdoc/>
    public async Task<string> RetrieveNameForIdAsync(int id)
    {
        return (
            (await _context.BusinessUnits.FirstOrDefaultAsync(bu => bu.Id == id))
            ?? throw new BusinessUnitNotFoundException(id)
        ).BusinessUnitName;
    }

    /// <inheritdoc/>
    public async Task<BusinessUnit> GetBusinessUnitByNameAsync(string buName)
    {
        return await _context.BusinessUnits.FirstOrDefaultAsync(bu => bu.BusinessUnitName == buName)
            ?? throw new BusinessUnitNotFoundException(buName);
    }

    /// <inheritdoc/>
    public async Task AddBusinessUnitAsync(BusinessUnit bu)
    {
        if (!await GetIf(b => b.Id == bu.Id).AnyAsync())
        {
            _ = _context.BusinessUnits.Add(bu);
        }
    }

    /// <inheritdoc/>
    public async Task<BusinessUnit> UpdateBusinessUnitAsync(BusinessUnit businessUnit)
    {
        if (!await CheckIfBusinessUnitExistsAsync(businessUnit.Id))
        {
            throw new BusinessUnitNotFoundException(businessUnit.Id);
        }
        _ = _context.BusinessUnits.Update(businessUnit);
        return businessUnit;
    }

    /// <inheritdoc/>
    public async Task<BusinessUnit> DeleteBusinessUnitAsync(BusinessUnit businessUnit)
    {
        _ = _context.BusinessUnits.Remove(businessUnit);
        return await Task.FromResult(businessUnit);
    }

    /// <inheritdoc/>
    public async Task<BusinessUnit> GetBusinessUnitWithTeamsAsync(int id)
    {
        return await _context
                .BusinessUnits.Include(bu => bu.Teams)
                .FirstOrDefaultAsync(bu => bu.Id == id)
            ?? throw new BusinessUnitNotFoundException(id);
    }
}
