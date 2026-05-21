using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.BusinessUnits;
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
}
