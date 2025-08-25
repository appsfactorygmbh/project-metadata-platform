using MediatR;

namespace ProjectMetadataPlatform.Application.Teams;

/// <summary>
/// Command to create a new Team with the given attributes.
/// </summary>
/// <param name="TeamName">The name of the new team.</param>
/// <param name="BusinessUnit">The BU of the new team.</param>
/// <param name="PTL">The PTL responsible for the new team.</param>
public record CreateTeamCommand(string TeamName, string BusinessUnit, string? PTL) : IRequest<int>;
