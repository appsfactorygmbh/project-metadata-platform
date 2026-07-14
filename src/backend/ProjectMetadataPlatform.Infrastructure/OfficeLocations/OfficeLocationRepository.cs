using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.OfficeLocationExceptions;
using ProjectMetadataPlatform.Domain.OfficeLocations;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.OfficeLocations;

/// <summary>
/// The repository for office locations that handles the data access.
/// </summary>
public class OfficeLocationRepository : RepositoryBase<OfficeLocation>, IOfficeLocationRepository
{
    private readonly ProjectMetadataPlatformDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="OfficeLocationRepository" /> class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing office location data.</param>
    public OfficeLocationRepository(ProjectMetadataPlatformDbContext dbContext)
        : base(dbContext)
    {
        _context = dbContext;
    }

    /// <inheritdoc/>
    public async Task<IQueryable<OfficeLocation>> GetOfficeLocationsAsync()
    {
        return _context.OfficeLocations.AsNoTracking();
    }

    /// <inheritdoc/>
    public async Task<OfficeLocation> GetOfficeLocationAsync(int id)
    {
        return await _context.OfficeLocations.FirstOrDefaultAsync(location => location.Id == id)
            ?? throw new OfficeLocationNotFoundException(id);
    }

    /// <inheritdoc/>
    public async Task<bool> CheckIfOfficeLocationNameExistsAsync(string officeLocationName)
    {
        return await _context.OfficeLocations.AnyAsync(officeLocation =>
            officeLocation.OfficeLocationName == officeLocationName
        );
    }

    /// <inheritdoc/>
    public async Task<bool> CheckIfOfficeLocationExistsAsync(int id)
    {
        return await _context.OfficeLocations.AnyAsync(officeLocation => officeLocation.Id == id);
    }

    /// <inheritdoc/>
    public async Task<OfficeLocation> GetOfficeLocationByNameAsync(string officeLocationName)
    {
        return await _context.OfficeLocations.FirstOrDefaultAsync(o =>
                o.OfficeLocationName == officeLocationName
            ) ?? throw new OfficeLocationNotFoundException(officeLocationName);
    }

    /// <inheritdoc/>
    public async Task AddOfficeLocationAsync(OfficeLocation location)
    {
        if (!await GetIf(o => o.Id == location.Id).AnyAsync())
        {
            _ = _context.OfficeLocations.Add(location);
        }
    }

    /// <inheritdoc/>
    public async Task<OfficeLocation> UpdateOfficeLocationAsync(OfficeLocation location)
    {
        if (!await CheckIfOfficeLocationExistsAsync(location.Id))
        {
            throw new OfficeLocationNotFoundException(location.Id);
        }
        _ = _context.OfficeLocations.Update(location);
        return location;
    }

    /// <inheritdoc/>
    public async Task<OfficeLocation> DeleteOfficeLocationAsync(OfficeLocation location)
    {
        _ = _context.OfficeLocations.Remove(location);
        return await Task.FromResult(location);
    }
}
