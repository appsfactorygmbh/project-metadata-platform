using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Departments;
using ProjectMetadataPlatform.Domain.Errors.DepartmentExceptions;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Departments;

public class DepartmentRepository : RepositoryBase<Department>, IDepartmentRepository
{
    private readonly ProjectMetadataPlatformDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DepartmentRepository" /> class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing department data.</param>
    public DepartmentRepository(ProjectMetadataPlatformDbContext dbContext)
        : base(dbContext)
    {
        _context = dbContext;
    }
    /// <inheritdoc/>
    public async Task<bool> CheckIfDepartmentNameExistsAsync(string departmentName)
    {
        return await _context.Departments.AnyAsync(department =>
            department.DepartmentName == departmentName
        );
    }
    /// <inheritdoc/>
    public async Task<Department> GetDepartmentByNameAsync(string departmentName)
    {
        return await _context.Departments.FirstOrDefaultAsync(d =>
                d.DepartmentName == departmentName
            ) ?? throw new DepartmentNotFoundException(departmentName);
    }

    /// <inheritdoc/>
    public async Task AddDepartmentAsync(Department department)
    {
        if (!await GetIf(d => d.Id == department.Id).AnyAsync())
        {
            _context.Departments.Add(department);
        }
    }
}
