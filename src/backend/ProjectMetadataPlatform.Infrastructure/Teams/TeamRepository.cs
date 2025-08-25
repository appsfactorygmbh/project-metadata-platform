using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
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
    public async Task<List<Team>> GetTeamsAsync(string? fullTextQuery, string? teamName)
    {
        var filteredQuery = _context.Teams.AsQueryable();
        if (!string.IsNullOrWhiteSpace(fullTextQuery))
        {
            var lowerTextSearch = fullTextQuery.ToLowerInvariant();
            filteredQuery = filteredQuery.Where(team =>
                EF.Functions.Like(team.BusinessUnit.ToLower(), $"%{lowerTextSearch}%")
                || (
                    team.PTL != null
                    && EF.Functions.Like(team.PTL.ToLower(), $"%{lowerTextSearch}%")
                )
                || EF.Functions.Like(team.TeamName.ToLower(), $"%{lowerTextSearch}%")
            );
        }
        if (!string.IsNullOrWhiteSpace(teamName))
        {
            filteredQuery = filteredQuery.Where(team =>
                EF.Functions.Like(team.TeamName.ToLower(), $"%{teamName.ToLower()}%")
            );
        }
        return await filteredQuery.ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Team> GetTeamAsync(int id)
    {
        return await _context.Teams.FirstOrDefaultAsync(team => team.Id == id)
            ?? throw new TeamNotFoundException(id);
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
            _context.Teams.Add(team);
        }
    }

    /// <inheritdoc/>
    public async Task<Team> DeleteTeamAsync(Team team)
    {
        _context.Teams.Remove(team);
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
        _context.Teams.Update(team);
        return team;
    }

    /// <inheritdoc/>
    public async Task<Team> GetTeamWithProjectsAsync(int id)
    {
        return await _context
                .Teams.Include(team => team.Projects)
                .FirstOrDefaultAsync(team => team.Id == id) ?? throw new TeamNotFoundException(id);
    }
}
