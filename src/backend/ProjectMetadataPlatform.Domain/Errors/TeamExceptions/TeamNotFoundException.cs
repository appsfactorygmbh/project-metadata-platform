using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.PluginExceptions;

/// <summary>
/// Exception thrown when a team is not found.
/// </summary>
public class TeamNotFoundException : EntityNotFoundException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TeamNotFoundException"/> class with a specified team ID.
    /// </summary>
    /// <param name="teamId">The ID of the team that was not found.</param>
    public TeamNotFoundException(int teamId)
        : base("The team with id " + teamId + " was not found.") { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TeamNotFoundException"/> class with a specified team name.
    /// </summary>
    /// <param name="teamName">The name of the team that was not found.</param>
    public TeamNotFoundException(string teamName)
        : base("The team with name " + teamName + " was not found.") { }
}
