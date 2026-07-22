using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Teams;

/// <inheritdoc />
public class GetAllTeamsQueryHandler
    : IRequestHandler<
        GetAllTeamsQuery,
        (IEnumerable<Team>, IEnumerable<AuthorizationConstants.Actions>)
    >
{
    private readonly ITeamRepository _teamRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="GetAllTeamsQueryHandler" />.
    /// </summary>
    public GetAllTeamsQueryHandler(
        ITeamRepository teamRepository,
        IAuthorizationService authorizationService
    )
    {
        _teamRepository = teamRepository;
        _authorizationService = authorizationService;
    }

    /// <inheritdoc />
    public async Task<(IEnumerable<Team>, IEnumerable<AuthorizationConstants.Actions>)> Handle(
        GetAllTeamsQuery request,
        CancellationToken cancellationToken
    )
    {
        var teams = await _teamRepository.GetTeamsAsync(
            fullTextQuery: request.FullTextQuery,
            teamName: request.TeamName
        );

        var queriedteams = await _authorizationService.TryGetPlanResourceQuery(teams);
        var permissions = await _authorizationService.GetPermissions<Team>(            actions: [AuthorizationConstants.Actions.CREATE]);
        if (queriedteams == null)
        {
            List<Team> filteredteams = [];
            foreach (var team in teams)
            {
                if (
                    await _authorizationService.CheckAccess(
                        team,
                        AuthorizationConstants.Actions.GET
                    )
                )
                {
                    filteredteams.Add(team);
                }
            }
            return (filteredteams.OrderBy(team => team.TeamName.ToLowerInvariant()), permissions);
        }
        return (
            (await queriedteams.ToListAsync(cancellationToken: cancellationToken)).OrderBy(team =>
                team.TeamName.ToLowerInvariant()
            ),
            permissions
        );
    }
}
