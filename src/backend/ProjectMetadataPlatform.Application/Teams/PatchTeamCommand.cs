using MediatR;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Teams;

/// <summary>
/// Represents a command to update a team.
/// </summary>
/// <param name="Id">The unique identifier of the team to be patched.</param>
/// <param name="TeamName">The name of the team.</param>
/// <param name="PTL">The PTL of the team.</param>
/// <param name="BusinessUnit">The BU of the team.</param>
/// <returns>A team object that represents the updated team.</returns>
public record PatchTeamCommand(
    int Id,
    string? TeamName = null,
    string? PTL = null,
    string? BusinessUnit = null
) : IRequest<Team>;
