using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.PluginExceptions;

/// <summary>
/// Exception thrown when a team is not found.
/// </summary>
/// <param name="teamId">Id of the team that was searched for.</param>
public class TeamNotFoundException(int teamId)
    : EntityNotFoundException("The team with id " + teamId + " was not found.") { }
