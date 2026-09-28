using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Teams;
using ProjectMetadataPlatform.Domain.Errors.TeamExceptions;
using ProjectMetadataPlatform.Domain.Teams;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Teams;

/// <summary>
/// The repository for users that handles the data access.
/// </summary>
public class TeamRepository : RepositoryBase<Team>, ITeamRepository
{
    private readonly ProjectMetadataPlatformDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="TeamRepository" /> class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing project data.</param>
    public TeamRepository(ProjectMetadataPlatformDbContext dbContext)
        : base(dbContext)
    {
        _context = dbContext;
    }

    /// <inheritdoc/>
    public async Task<IQueryable<Team>> GetTeamsAsync(
        string? fullTextQuery,
        string? teamName,
        TeamCursor? cursor
    )
    {
        var filteredQuery = GetEverything();
        if (cursor != null)
        {
            filteredQuery = filteredQuery.Where(t => t.TeamName.CompareTo(cursor.TeamName) > 0);
        }
        if (!string.IsNullOrWhiteSpace(fullTextQuery))
        {
            filteredQuery = filteredQuery.Where(team =>
                EF.Functions.ILike(team.BusinessUnit!.BusinessUnitName, $"%{fullTextQuery}%")
                || (team.PTL != null && EF.Functions.ILike(team.PTL, $"%{fullTextQuery}%"))
                || EF.Functions.ILike(team.TeamName, $"%{fullTextQuery}%")
            );
        }
        if (!string.IsNullOrWhiteSpace(teamName))
        {
            filteredQuery = filteredQuery.Where(team =>
                EF.Functions.ILike(team.TeamName, $"%{teamName}%")
            );
        }
        return filteredQuery
            .Include(t => t.BusinessUnit)
                .ThenInclude(b => b!.Users!)
                    .ThenInclude(u => u.Departments)
            .OrderBy(t => t.TeamName);
    }

    /// <inheritdoc/>
    public async Task<Team> GetTeamAsync(int id)
    {
        return await _context
                .Teams.Include(t => t.BusinessUnit)
                .FirstOrDefaultAsync(team => team.Id == id)
            ?? throw new TeamNotFoundException(id);
    }

    /// <inheritdoc/>
    public async Task<Team> GetTeamByNameAsync(string teamName)
    {
        return await _context
                .Teams.Include(t => t.BusinessUnit)
                .FirstOrDefaultAsync(team => team.TeamName == teamName)
            ?? throw new TeamNotFoundException(teamName: teamName);
    }

    /// <inheritdoc/>
    public async Task<bool> CheckIfTeamExistsAsync(int id)
    {
        return await _context.Teams.AnyAsync(team => team.Id == id);
    }

    /// <inheritdoc/>
    public async Task<string> RetrieveNameForIdAsync(int id)
    {
        return (
            (await _context.Teams.FirstOrDefaultAsync(team => team.Id == id))
            ?? throw new TeamNotFoundException(id)
        ).TeamName;
    }

    /// <inheritdoc/>
    public async Task AddTeamAsync(Team team)
    {
        if (!await GetIf(p => p.Id == team.Id).AnyAsync())
        {
            _ = _context.Teams.Add(team);
        }
    }

    /// <inheritdoc/>
    public async Task<Team> DeleteTeamAsync(Team team)
    {
        _ = _context.Teams.Remove(team);
        return await Task.FromResult(team);
    }

    /// <inheritdoc/>
    public async Task<bool> CheckIfTeamNameExistsAsync(string name)
    {
        return await _context.Teams.AnyAsync(team => team.TeamName == name);
    }

    /// <inheritdoc/>
    public async Task<Team> UpdateTeamAsync(Team team)
    {
        if (!await CheckIfTeamExistsAsync(team.Id))
        {
            throw new TeamNotFoundException(team.Id);
        }
        _ = _context.Teams.Update(team);
        return team;
    }

    /// <inheritdoc/>
    public async Task<Team> GetTeamWithProjectsAsync(int id)
    {
        return await _context
                .Teams.Include(t => t.BusinessUnit)
                .Include(team => team.Projects)
                .FirstOrDefaultAsync(team => team.Id == id)
            ?? throw new TeamNotFoundException(id);
    }
}
