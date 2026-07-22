using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Teams;

/// <inheritdoc />
public class GetTeamQueryHandler
    : IRequestHandler<GetTeamQuery, (Team, IEnumerable<AuthorizationConstants.Actions>)>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="GetTeamQueryHandler" />.
    /// </summary>
    public GetTeamQueryHandler(
        ITeamRepository teamRepository,
        IAuthorizationService authorizationService
    )
    {
        _teamRepository = teamRepository;
        _authorizationService = authorizationService;
    }

    /// <inheritdoc/>
    public async Task<(Team, IEnumerable<AuthorizationConstants.Actions>)> Handle(
        GetTeamQuery request,
        CancellationToken cancellationToken
    )
    {
        var team = await _teamRepository.GetTeamAsync(request.Id);
        if (!await _authorizationService.CheckAccess(team, AuthorizationConstants.Actions.GET))
        {
            throw new UnauthorizedException();
        }
        var permissions = await _authorizationService.GetPermissions(team,[AuthorizationConstants.Actions.EDIT,AuthorizationConstants.Actions.DELETE]);
        return (team, permissions);
    }
}
