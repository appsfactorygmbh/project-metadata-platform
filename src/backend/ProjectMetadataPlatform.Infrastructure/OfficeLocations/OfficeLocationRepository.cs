using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Departments;
using ProjectMetadataPlatform.Domain.Errors.OfficeLocationExceptions;
using ProjectMetadataPlatform.Domain.OfficeLocations;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.OfficeLocations;

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
    public async Task<bool> CheckIfOfficeLocationNameExistsAsync(string officeLocationName)
    {
        return await _context.OfficeLocations.AnyAsync(officeLocation =>
            officeLocation.OfficeLocationName == officeLocationName
        );
    }
    /// <inheritdoc/>
    public async Task<OfficeLocation> GetOfficeLocationByNameAsync(string officeLocationName)
    {
        return await _context.OfficeLocations.FirstOrDefaultAsync(o =>
                o.OfficeLocationName == officeLocationName
            ) ?? throw new OfficeLocationNotFoundException(officeLocationName);
    }

    /// <inheritdoc/>
    public async Task AddOfficeLocationAsync(OfficeLocation officeLocation)
    {
        if (!await GetIf(o => o.Id == officeLocation.Id).AnyAsync())
        {
            _context.OfficeLocations.Add(officeLocation);
        }
    }
}
