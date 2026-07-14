using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.TeamExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Teams;

/// <summary>
/// Handler for the <see cref="DeleteGlobalPluginCommand"/>
/// </summary>
public class DeleteTeamCommandHandler : IRequestHandler<DeleteTeamCommand>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of<see cref="DeleteTeamCommandHandler"/>.
    /// </summary>
    /// <param name="teamRepository"></param>
    /// <param name="logRepository"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="authorizationService"></param>
    public DeleteTeamCommandHandler(
        ITeamRepository teamRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService
    )
    {
        _teamRepository = teamRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Delete team with the given id.
    /// </summary>
    /// <param name="request">The request that needs to be handled.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The response of the request.</returns>
    public async Task Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
    {
        var teamToDelete = await _teamRepository.GetTeamWithProjectsAsync(request.Id);
        if (
            !await _authorizationService.CheckAccess(
                teamToDelete,
                AuthorizationConstants.Actions.DELETE
            )
        )
        {
            throw new UnauthorizedException();
        }
        if (teamToDelete.Projects != null && teamToDelete.Projects.Count > 0)
        {
            throw new TeamStillLinkedToProjectsException(
                teamToDelete,
                [.. teamToDelete.Projects.Select(proj => proj.Id)]
            );
        }

        _ = await _teamRepository.DeleteTeamAsync(teamToDelete);

        var changes = new List<LogChange>();

        await _logRepository.AddTeamLogForCurrentActor(teamToDelete, Action.REMOVED_TEAM, changes);
        await _unitOfWork.CompleteAsync();
    }
}
