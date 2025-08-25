namespace ProjectMetadataPlatform.Api.Teams.Models;

/// <summary>
/// Request for creating a new team.
/// </summary>
/// <param name="TeamName">The name of the new team.</param>
/// <param name="BusinessUnit">The BU of the new team.</param>
/// <param name="PTL">The PTL responsible for the new team.</param>
public record CreateTeamRequest(string TeamName, string BusinessUnit, string? PTL);
