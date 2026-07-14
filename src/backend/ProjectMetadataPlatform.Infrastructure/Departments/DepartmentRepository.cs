using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Departments;
using ProjectMetadataPlatform.Domain.Errors.DepartmentExceptions;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Departments;

/// <summary>
/// The repository for departments that handles the data access.
/// </summary>
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
    public async Task<IQueryable<Department>> GetDepartmentsAsync()
    {
        return _context.Departments.AsNoTracking();
    }

    /// <inheritdoc/>
    public async Task<Department> GetDepartmentAsync(int id)
    {
        return await _context.Departments.FirstOrDefaultAsync(department => department.Id == id)
            ?? throw new DepartmentNotFoundException(id);
    }

    /// <inheritdoc/>
    public async Task<bool> CheckIfDepartmentNameExistsAsync(string departmentName)
    {
        return await _context.Departments.AnyAsync(department =>
            department.DepartmentName == departmentName
        );
    }

    /// <inheritdoc/>
    public async Task<bool> CheckIfDepartmentExistsAsync(int id)
    {
        return await _context.Departments.AnyAsync(department => department.Id == id);
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
            _ = _context.Departments.Add(department);
        }
    }

    /// <inheritdoc/>
    public async Task<Department> UpdateDepartmentAsync(Department department)
    {
        if (!await CheckIfDepartmentExistsAsync(department.Id))
        {
            throw new DepartmentNotFoundException(department.Id);
        }
        _ = _context.Departments.Update(department);
        return department;
    }

    /// <inheritdoc/>
    public async Task<Department> DeleteDepartmentAsync(Department department)
    {
        _ = _context.Departments.Remove(department);
        return await Task.FromResult(department);
    }
}
