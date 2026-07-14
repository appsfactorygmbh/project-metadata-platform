using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

/// <summary>
/// Handler for the <see cref="GetLinkedTeamsQuery" />.
/// </summary>
public class GetLinkedTeamsQueryHandler : IRequestHandler<GetLinkedTeamsQuery, List<int>>
{
    private readonly IBusinessUnitRepository _businessUnitRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="GetLinkedTeamsQueryHandler" />.
    /// </summary>
    public GetLinkedTeamsQueryHandler(
        IBusinessUnitRepository businessUnitRepository,
        IAuthorizationService authorizationService
    )
    {
        _businessUnitRepository = businessUnitRepository;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles the request to return linked teams.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of Team Id's.</returns>
    public async Task<List<int>> Handle(
        GetLinkedTeamsQuery request,
        CancellationToken cancellationToken
    )
    {
        var bu = await _businessUnitRepository.GetBusinessUnitWithTeamsAsync(request.Id);
        if (!await _authorizationService.CheckAccess(bu, AuthorizationConstants.Actions.GET))
        {
            throw new UnauthorizedException();
        }
        return [.. (bu.Teams ?? []).Select(team => team.Id)];
    }
}
