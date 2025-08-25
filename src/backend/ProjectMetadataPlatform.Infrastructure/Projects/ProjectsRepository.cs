using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
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
    /// <returns>A task representing the asynchronous operation. When this task completes, it returns a collection of projects.</returns>
    public async Task<IEnumerable<Project>> GetProjectsAsync(GetAllProjectsQuery query)
    {
        var filteredQuery = _context.Projects.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var lowerTextSearch = query.Search.ToLowerInvariant();

            filteredQuery = filteredQuery.Where(project =>
                EF.Functions.Like(project.ProjectName.ToLower(), $"%{lowerTextSearch}%")
                || EF.Functions.Like(project.ClientName.ToLower(), $"%{lowerTextSearch}%")
                || (
                    project.Team != null
                    && EF.Functions.Like(
                        project.Team.BusinessUnit.ToLower(),
                        $"%{lowerTextSearch}%"
                    )
                )
                || (
                    project.Team != null
                    && EF.Functions.Like(project.Team.TeamName.ToLower(), $"%{lowerTextSearch}%")
                )
                || EF.Functions.Like(project.Company.ToLower(), $"%{lowerTextSearch}%")
                || EF.Functions.Like(project.Notes.ToLower(), $"%{lowerTextSearch}%")
            );
        }

        if (query.Request != null)
        {
            if (!string.IsNullOrWhiteSpace(query.Request.ProjectName))
            {
                var lowerProjectNameSearch = query.Request.ProjectName.ToLower();
                filteredQuery = filteredQuery.Where(project =>
                    EF.Functions.Like(project.ProjectName.ToLower(), $"%{lowerProjectNameSearch}%")
                );
            }

            if (!string.IsNullOrWhiteSpace(query.Request.ClientName))
            {
                var lowerClientNameSearch = query.Request.ClientName.ToLower();
                filteredQuery = filteredQuery.Where(project =>
                    EF.Functions.Like(project.ClientName.ToLower(), $"%{lowerClientNameSearch}%")
                );
            }

            if (query.Request.BusinessUnit is { Count: > 0 })
            {
                var lowerBusinessUnits = query
                    .Request.BusinessUnit.Select(bu => bu.ToLower())
                    .ToList();
                filteredQuery = filteredQuery.Where(project =>
                    project.Team != null
                    && lowerBusinessUnits.Contains(project.Team.BusinessUnit.ToLower())
                );
            }

            if (query.Request.TeamName is { Count: > 0 })
            {
                var lowerTeamNames = query.Request.TeamName.Select(tn => tn.ToLower()).ToList();
                filteredQuery = filteredQuery.Where(project =>
                    project.Team != null && lowerTeamNames.Contains(project.Team.TeamName.ToLower())
                );
            }

            if (query.Request.IsArchived is not null)
            {
                filteredQuery = filteredQuery.Where(project =>
                    project.IsArchived == query.Request.IsArchived
                );
            }

            if (query.Request.Company is { Count: > 0 })
            {
                var lowerCompanies = query.Request.Company.Select(c => c.ToLower()).ToList();
                filteredQuery = filteredQuery.Where(project =>
                    lowerCompanies.Contains(project.Company.ToLower())
                );
            }

            if (query.Request.IsmsLevel is not null)
            {
                filteredQuery = filteredQuery.Where(project =>
                    project.IsmsLevel == query.Request.IsmsLevel
                );
            }
        }

        return await filteredQuery.Include(p => p.Team).ToListAsync();
    }

    /// <summary>
    /// Asynchronously retrieves all projects from the database.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. When this task completes, it returns a collection of projects.</returns>
    public async Task<IEnumerable<Project>> GetProjectsAsync()
    {
        return await _context.Projects.AsNoTracking().Include(p => p.Team).ToListAsync();
    }

    /// <summary>
    /// Asynchronously retrieves a project from the database by its identifier.
    /// </summary>
    /// <param name="id">Identification number for a project.</param>
    /// <returns>A task representing the asynchronous operation. When this task completes, it returns one project.</returns>
    public async Task<Project> GetProjectAsync(int id)
    {
        return await GetIf(p => p.Id == id).Include(proj => proj.Team).FirstOrDefaultAsync()
            ?? throw new ProjectNotFoundException(id);
    }

    /// <inheritdoc />
    public async Task<Project> GetProjectWithPluginsAsync(int id)
    {
        return await GetIf(p => p.Id == id)
                .Include(p => p.ProjectPlugins)
                .Include(p => p.Team)
                .FirstOrDefaultAsync() ?? throw new ProjectNotFoundException(id);
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
                .FirstOrDefaultAsync() ?? throw new ProjectNotFoundException(slug);
    }
}
