using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.BusinessUnits;

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

    public async Task<bool> CheckIfBusinessUnitExistsAsync(int id)
    {
        return await _context.BusinessUnits.AnyAsync(bu => bu.Id == id);
    }

    public async Task<bool> CheckIfBusinessUnitNameExistsAsync(string buName)
    {
        return await _context.BusinessUnits.AnyAsync(bu => bu.BusinessUnitName == buName);
    }

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
            _context.BusinessUnits.Add(bu);
        }
    }
}
