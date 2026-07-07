using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;

namespace ProjectMetadataPlatform.Application.Teams;

/// <inheritdoc />
public class GetLinkedProjectsQueryHandler : IRequestHandler<GetLinkedProjectsQuery, List<string>>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="GetTeamQueryHandler" />.
    /// </summary>
    public GetLinkedProjectsQueryHandler(
        ITeamRepository teamRepository,
        IAuthorizationService authorizationService
    )
    {
        _teamRepository = teamRepository;
        _authorizationService = authorizationService;
    }

    /// <inheritdoc/>
    public async Task<List<string>> Handle(
        GetLinkedProjectsQuery request,
        CancellationToken cancellationToken
    )
    {
        var team = await _teamRepository.GetTeamWithProjectsAsync(request.Id);
        if (
            !(await _authorizationService.CheckAccess(team, [AuthorizationConstants.Actions.GET]))[
                AuthorizationConstants.Actions.GET
            ]
        )
        {
            throw new UnauthorizedException();
        }
        return [.. (team.Projects ?? []).Select(proj => proj.Slug)];
    }
}
