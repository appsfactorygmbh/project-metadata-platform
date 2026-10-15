using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Teams;

/// <inheritdoc />
public class GetAllTeamsQueryHandler
    : IRequestHandler<
        GetAllTeamsQuery,
        (IEnumerable<Team>, IEnumerable<AuthorizationConstants.Actions>, TeamCursor?)
    >
{
    private readonly ITeamRepository _teamRepository;
    private readonly IAuthorizationService _authorizationService;

    private readonly IPaginationHelper _paginationHelper;

    /// <summary>
    /// Creates a new instance of <see cref="GetAllTeamsQueryHandler" />.
    /// </summary>
    public GetAllTeamsQueryHandler(
        ITeamRepository teamRepository,
        IAuthorizationService authorizationService,
        IPaginationHelper paginationHelper
    )
    {
        _teamRepository = teamRepository;
        _authorizationService = authorizationService;
        _paginationHelper = paginationHelper;
    }

    /// <inheritdoc />
    public async Task<(
        IEnumerable<Team>,
        IEnumerable<AuthorizationConstants.Actions>,
        TeamCursor?
    )> Handle(GetAllTeamsQuery request, CancellationToken cancellationToken)
    {
        var teams = await _teamRepository.GetTeamsAsync(
            fullTextQuery: request.FullTextQuery,
            teamName: request.TeamName,
            request.Cursor
        );
        var (paginatedTeams, lastTeam) = await _paginationHelper.PaginateWithAuthAsync(
            teams,
            request.Limit
        );

        var permissions = await _authorizationService.GetAllowedActions<Team>(
            actions: [AuthorizationConstants.Actions.CREATE]
        );
        var nextCursor = lastTeam == null ? null : new TeamCursor(lastTeam.TeamName);
        return (paginatedTeams, permissions, nextCursor);
    }
}
