namespace ProjectMetadataPlatform.Api.Teams.Models;

/// <summary>
/// Represents a request to patch a global plugin.
/// </summary>
/// <param name="TeamName">The name of the team. Null if not being updated.</param>
/// <param name="PTL">The PTL of the team. Null if not being updated.</param>
/// <param name="BusinessUnit">The BU of the team. Null if not being updated.</param>
public record PatchTeamRequest(
    string? TeamName = null,
    string? PTL = null,
    string? BusinessUnit = null
);
