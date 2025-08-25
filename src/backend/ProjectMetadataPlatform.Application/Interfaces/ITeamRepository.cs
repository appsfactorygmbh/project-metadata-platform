using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Repository for authentication.
/// </summary>
public interface ITeamRepository
{
    /// <summary>
    /// Retrieves all teams matching the given filtering options.
    /// </summary>
    /// <param name="fullTextQuery">Optional. Full text search over all attributes of a team except the id.</param>
    /// <param name="teamName">Optional. The name of the team to filter by.</param>
    /// <returns></returns>
    Task<List<Team>> GetTeamsAsync(string? fullTextQuery, string? teamName);

    /// <summary>
    /// Retrieves the team by the given id.
    /// </summary>
    /// <param name="id">Id of the team.</param>
    /// <returns>The team specified by the given id.</returns>
    Task<Team> GetTeamAsync(int id);

    /// <summary>
    /// Retrieves the team by the given id.
    /// </summary>
    /// <param name="id">Id of the team.</param>
    /// <returns>The team specified by the given id.</returns>
    Task<Team> GetTeamWithProjectsAsync(int id);

    /// <summary>
    /// Retrieves the name of the team specified by the given id.
    /// </summary>
    /// <param name="id">Id of the team.</param>
    /// <returns>The name of the team specified by the id.</returns>
    Task<string> RetrieveNameForIdAsync(int id);

    /// <summary>
    /// Checks if a team exists.
    /// </summary>
    /// <param name="id">Id of the team.</param>
    /// <returns>True if the team with the given id is present.</returns>
    Task<bool> CheckIfTeamExistsAsync(int id);

    /// <summary>
    /// Checks if a team with this name exists case insensitive.
    /// </summary>
    /// <param name="name">Name of the team.</param>
    /// <returns>True if the team with the given name is present.</returns>
    Task<bool> CheckIfTeamNameExistsAsync(string name);

    /// <summary>
    /// Saves a team to the database.
    /// </summary>
    /// <param name="team">The team to add.</param>
    /// <returns>A task representing the asynchronous operation to add the team.</returns>
    Task AddTeamAsync(Team team);

    /// <summary>
    /// Overwrites a team in the database.
    /// </summary>
    /// <param name="team">The team to overwrite.</param>
    /// <returns>The updated team.</returns>
    Task<Team> UpdateTeamAsync(Team team);

    /// <summary>
    /// Deletes a team from the database.
    /// </summary>
    /// <param name="team">The team to delete.</param>
    /// <returns>A task representing the asynchronous operation, which upon completion returns the deleted team.</returns>
    Task<Team> DeleteTeamAsync(Team team);
}
