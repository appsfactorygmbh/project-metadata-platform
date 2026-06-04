namespace ProjectMetadataPlatform.Api.Teams.Models;

/// <summary>
/// Request for creating a new team.
/// </summary>
/// <param name="TeamName">The name of the new team.</param>
/// <param name="BusinessUnitId">The Id of the BU of the new team.</param>
/// <param name="PTL">The PTL responsible for the new team.</param>
public record CreateTeamRequest(string TeamName, int BusinessUnitId, string? PTL);
