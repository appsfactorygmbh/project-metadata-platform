using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Common;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Projects;

/// <summary>
/// Repository for accessing and managing project data in the database.
/// </summary>
public class ProjectsRepository : RepositoryBase<Project>, IProjectsRepository
{
    private readonly ProjectMetadataPlatformDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectsRepository" /> class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing project data.</param>
    public ProjectsRepository(ProjectMetadataPlatformDbContext dbContext)
        : base(dbContext)
    {
        _context = dbContext;
    }

    /// <summary>
    /// Asynchronously retrieves all projects with specific search pattern or filter matches from the database.
    /// </summary>
    /// <param name="query">The query containing filters and search pattern.</param>
    /// <param name="cursor">Optional Cursor for pagination.</param>
    /// <param name="canSearchPlugin">Whether Plugin Attributes are allowed to be filtered.</param>
    /// <param name="canSearchBilling">Whether Billing Attributes are allowed to be filtered.</param>
    /// <returns>A collection of projects.</returns>
    public async Task<IQueryable<Project>> GetProjectsAsync(
        GetAllProjectsQuery query,
        ProjectCursor? cursor,
        bool canSearchPlugin = false,
        bool canSearchBilling = false
    )
    {
        var joinQuery = GetEverything()
            .Include(p => p.Team)
                .ThenInclude(t => t!.BusinessUnit)
            .Include(p => p.Company)
            .Join(
                _context.ProjectSearchIndex,
                p => p.Id,
                s => s.Id,
                (p, s) => new { Project = p, Search = s }
            );

        if (query.Request != null)
        {
            if (!string.IsNullOrWhiteSpace(query.Request.ProjectName))
            {
                joinQuery = joinQuery.Where(x =>
                    EF.Functions.ILike(
                        x.Search.ProjectName,
                        $"%{query.Request.ProjectName.Trim()}%"
                    )
                );
            }

            if (!string.IsNullOrWhiteSpace(query.Request.ClientName))
            {
                joinQuery = joinQuery.Where(x =>
                    EF.Functions.ILike(x.Search.ClientName, $"%{query.Request.ClientName.Trim()}%")
                );
            }

            if (query.Request.BusinessUnit?.Any() == true)
            {
                var lowerBUs = query.Request.BusinessUnit.Select(b => b.ToLower()).ToList();
                joinQuery = joinQuery.Where(x =>
                    x.Search.BusinessUnitName != null
                    && lowerBUs.Contains(x.Search.BusinessUnitName)
                );
            }

            if (query.Request.TeamName?.Any() == true)
            {
                var lowerTeams = query.Request.TeamName.Select(t => t.ToLower()).ToList();
                joinQuery = joinQuery.Where(x =>
                    x.Search.TeamName != null && lowerTeams.Contains(x.Search.TeamName)
                );
            }

            if (query.Request.Company?.Any() == true)
            {
                var lowerCompanies = query.Request.Company.Select(c => c.ToLower()).ToList();
                joinQuery = joinQuery.Where(x =>
                    x.Search.CompanyName != null && lowerCompanies.Contains(x.Search.CompanyName)
                );
            }

            if (query.Request.IsArchived.HasValue)
            {
                joinQuery = joinQuery.Where(x =>
                    x.Search.IsArchived == query.Request.IsArchived.Value
                );
            }

            if (query.Request.IsEoC.HasValue)
            {
                joinQuery = joinQuery.Where(x => x.Search.IsEoC == query.Request.IsEoC.Value);
            }

            if (query.Request.IsmsLevel.HasValue)
            {
                joinQuery = joinQuery.Where(x =>
                    x.Project.IsmsLevel == query.Request.IsmsLevel.Value
                );
            }
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = $"%{query.Search.Trim()}%";
            var isArchivedSearch = nameof(Project.IsArchived)
                .Contains(query.Search.Trim(), System.StringComparison.InvariantCultureIgnoreCase);
            var isEoCSearch = nameof(Project.IsEoC)
                .Contains(query.Search.Trim(), System.StringComparison.InvariantCultureIgnoreCase);
            joinQuery = joinQuery.Where(x =>
                EF.Functions.ILike(x.Search.ProjectName, term)
                || EF.Functions.ILike(x.Search.ClientName, term)
                || EF.Functions.ILike(x.Search.CompanyName ?? "", term)
                || (canSearchPlugin && EF.Functions.ILike(x.Search.PluginsSearchText, term))
                || EF.Functions.ILike(x.Search.Notes ?? "", term)
                || EF.Functions.ILike(x.Search.CompanyStateText, term)
                || EF.Functions.ILike(x.Search.SecurityLevelText, term)
                || (isArchivedSearch && x.Search.IsArchived)
                || (isEoCSearch && x.Search.IsEoC)
                || (canSearchBilling && EF.Functions.ILike(x.Search.BillingSearchText, term))
            );
        }

        var sortBy = query.Request?.SortAttribute ?? ProjectSortCharacteristic.ClientName;
        var desc = query.Request?.SortOrder == SortOrder.DESC;

        if (cursor != null)
        {
            joinQuery = sortBy switch
            {
                ProjectSortCharacteristic.Company => desc
                    ? joinQuery.Where(x =>
                        (x.Search.CompanyName ?? "").CompareTo(cursor.CursorValue) < 0
                        || (
                            (x.Search.CompanyName ?? "") == cursor.CursorValue
                            && x.Search.Slug.CompareTo(cursor.Slug) < 0
                        )
                    )
                    : joinQuery.Where(x =>
                        (x.Search.CompanyName ?? "").CompareTo(cursor.CursorValue) > 0
                        || (
                            (x.Search.CompanyName ?? "") == cursor.CursorValue
                            && x.Search.Slug.CompareTo(cursor.Slug) > 0
                        )
                    ),

                ProjectSortCharacteristic.BusinessUnit => desc
                    ? joinQuery.Where(x =>
                        (x.Search.BusinessUnitName ?? "").CompareTo(cursor.CursorValue) < 0
                        || (
                            (x.Search.BusinessUnitName ?? "") == cursor.CursorValue
                            && x.Search.Slug.CompareTo(cursor.Slug) < 0
                        )
                    )
                    : joinQuery.Where(x =>
                        (x.Search.BusinessUnitName ?? "").CompareTo(cursor.CursorValue) > 0
                        || (
                            (x.Search.BusinessUnitName ?? "") == cursor.CursorValue
                            && x.Search.Slug.CompareTo(cursor.Slug) > 0
                        )
                    ),

                ProjectSortCharacteristic.Team => desc
                    ? joinQuery.Where(x =>
                        (x.Search.TeamName ?? "").CompareTo(cursor.CursorValue) < 0
                        || (
                            (x.Search.TeamName ?? "") == cursor.CursorValue
                            && x.Search.Slug.CompareTo(cursor.Slug) < 0
                        )
                    )
                    : joinQuery.Where(x =>
                        (x.Search.TeamName ?? "").CompareTo(cursor.CursorValue) > 0
                        || (
                            (x.Search.TeamName ?? "") == cursor.CursorValue
                            && x.Search.Slug.CompareTo(cursor.Slug) > 0
                        )
                    ),
                ProjectSortCharacteristic.ProjectName => desc
                    ? joinQuery.Where(x =>
                        x.Search.ProjectName.CompareTo(cursor.CursorValue) < 0
                        || (
                            x.Search.ProjectName == cursor.CursorValue
                            && x.Search.Slug.CompareTo(cursor.Slug) < 0
                        )
                    )
                    : joinQuery.Where(x =>
                        x.Search.ProjectName.CompareTo(cursor.CursorValue) > 0
                        || (
                            x.Search.ProjectName == cursor.CursorValue
                            && x.Search.Slug.CompareTo(cursor.Slug) > 0
                        )
                    ),
                ProjectSortCharacteristic.ClientName or _ => desc
                    ? joinQuery.Where(x =>
                        x.Search.ClientName.CompareTo(cursor.CursorValue) < 0
                        || (
                            x.Search.ClientName == cursor.CursorValue
                            && x.Search.Slug.CompareTo(cursor.Slug) < 0
                        )
                    )
                    : joinQuery.Where(x =>
                        x.Search.ClientName.CompareTo(cursor.CursorValue) > 0
                        || (
                            x.Search.ClientName == cursor.CursorValue
                            && x.Search.Slug.CompareTo(cursor.Slug) > 0
                        )
                    ),
            };
        }

        joinQuery = sortBy switch
        {
            ProjectSortCharacteristic.Company => desc
                ? joinQuery
                    .OrderByDescending(x => x.Search.CompanyName ?? "")
                    .ThenByDescending(x => x.Search.Slug)
                : joinQuery.OrderBy(x => x.Search.CompanyName ?? "").ThenBy(x => x.Search.Slug),

            ProjectSortCharacteristic.BusinessUnit => desc
                ? joinQuery
                    .OrderByDescending(x => x.Search.BusinessUnitName ?? "")
                    .ThenByDescending(x => x.Search.Slug)
                : joinQuery
                    .OrderBy(x => x.Search.BusinessUnitName ?? "")
                    .ThenBy(x => x.Search.Slug),

            ProjectSortCharacteristic.Team => desc
                ? joinQuery
                    .OrderByDescending(x => x.Search.TeamName ?? "")
                    .ThenByDescending(x => x.Search.Slug)
                : joinQuery.OrderBy(x => x.Search.TeamName ?? "").ThenBy(x => x.Search.Slug),
            ProjectSortCharacteristic.ProjectName => desc
                ? joinQuery
                    .OrderByDescending(x => x.Search.ProjectName)
                    .ThenByDescending(x => x.Search.Slug)
                : joinQuery.OrderBy(x => x.Search.ProjectName).ThenBy(x => x.Search.Slug),
            ProjectSortCharacteristic.ClientName or _ => desc
                ? joinQuery
                    .OrderByDescending(x => x.Search.ClientName)
                    .ThenByDescending(x => x.Search.Slug)
                : joinQuery.OrderBy(x => x.Search.ClientName).ThenBy(x => x.Search.Slug),
        };

        return joinQuery.Select(x => x.Project);
    }

    /// <summary>
    /// Asynchronously retrieves all projects from the database.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. When this task completes, it returns a collection of projects.</returns>
    public async Task<IEnumerable<Project>> GetProjectsAsync()
    {
        return await _context
            .Projects.AsNoTracking()
            .Include(p => p.Team)
                .ThenInclude(t => t!.BusinessUnit)
            .Include(p => p.Company)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Project> GetProjectAsync(int id)
    {
        return await GetIf(p => p.Id == id)
                .Include(p => p.ProjectPlugins!)
                    .ThenInclude(pp => pp.PluginBilling)
                .Include(p => p.ProjectPlugins!)
                    .ThenInclude(pp => pp.Plugin)
                .Include(p => p.Team)
                    .ThenInclude(t => t!.BusinessUnit)
                .Include(p => p.Company)
                .FirstOrDefaultAsync()
            ?? throw new ProjectNotFoundException(id);
    }

    /// <summary>
    /// Saves project to the database and returns it.
    /// </summary>
    /// <param name="project">Project to be saved in the database</param>
    /// <returns>Project is returned</returns>
    public async Task AddProjectAsync(Project project)
    {
        if (!await GetIf(p => p.Id == project.Id).AnyAsync())
        {
            Create(project);
        }
    }

    /// <summary>
    /// Checks if a project exists.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>True, if the project with the given id exists</returns>
    public async Task<bool> CheckProjectExists(int id)
    {
        return await _context.Projects.AnyAsync(project => project.Id == id);
    }

    /// <summary>
    /// Asynchronously deletes a project from the database.
    /// </summary>
    /// <param name="project">The project to delete.</param>
    /// <returns>A task representing the asynchronous operation, which upon completion returns the deleted project.</returns>
    public Task<Project> DeleteProjectAsync(Project project)
    {
        Delete(project);
        return Task.FromResult(project);
    }

    /// <inheritdoc/>
    public async Task<int> GetProjectIdBySlugAsync(string slug)
    {
        return await _context
                .Projects.Where(p => p.Slug == slug)
                .Select(p => (int?)p.Id)
                .FirstOrDefaultAsync()
            ?? throw new ProjectNotFoundException(slug);
    }
}
