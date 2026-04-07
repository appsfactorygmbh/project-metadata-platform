using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.PluginExceptions;

/// <summary>
/// Exception thrown when a team is not found.
/// </summary>
/// <param name="teamId">Id of the team that was searched for.</param>
public class TeamNotFoundException(int? teamId = null, string? teamName = null)
    : EntityNotFoundException(
        "The team "
            + (
                (teamId != null || teamName != null)
                    ? ((teamId != null) ? " with id " + teamId : "with name " + teamName)
                    : null
            )
            + " was not found."
    ) { }
