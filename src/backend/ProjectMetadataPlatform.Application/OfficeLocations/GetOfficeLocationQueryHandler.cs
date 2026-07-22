using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Application.OfficeLocations;

/// <summary>
/// Handler for the <see cref="GetOfficeLocationQuery" />.
/// </summary>
public class GetOfficeLocationQueryHandler
    : IRequestHandler<
        GetOfficeLocationQuery,
        (OfficeLocation, IEnumerable<AuthorizationConstants.Actions>)
    >
{
    private readonly IOfficeLocationRepository _officeLocationRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="GetOfficeLocationQueryHandler" />.
    /// </summary>
    public GetOfficeLocationQueryHandler(
        IOfficeLocationRepository officeLocationRepository,
        IAuthorizationService authorizationService
    )
    {
        _officeLocationRepository = officeLocationRepository;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles a Query to return an office location by id.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Office Location and allowed actions.</returns>
    public async Task<(OfficeLocation, IEnumerable<AuthorizationConstants.Actions>)> Handle(
        GetOfficeLocationQuery request,
        CancellationToken cancellationToken
    )
    {
        var location = await _officeLocationRepository.GetOfficeLocationAsync(request.Id);
        if (!await _authorizationService.CheckAccess(location, AuthorizationConstants.Actions.GET))
        {
            throw new UnauthorizedException();
        }
        var permissions = await _authorizationService.GetPermissions(location,[AuthorizationConstants.Actions.EDIT,AuthorizationConstants.Actions.DELETE]);
        return (location, permissions);
    }
}
