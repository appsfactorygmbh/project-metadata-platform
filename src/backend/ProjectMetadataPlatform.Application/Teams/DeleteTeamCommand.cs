using MediatR;

namespace ProjectMetadataPlatform.Application.Teams;

/// <summary>
/// Command to delete a new Plugin with the given id.
/// </summary>
/// <param name="Id">The id of the plugin to be removed.</param>
public record DeleteTeamCommand(int Id) : IRequest;
