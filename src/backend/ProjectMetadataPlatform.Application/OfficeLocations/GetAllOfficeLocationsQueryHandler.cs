using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Application.OfficeLocations;

/// <summary>
/// Handler for the <see cref="GetAllOfficeLocationsQuery" />.
/// </summary>
public class GetAllOfficeLocationsQueryHandler
    : IRequestHandler<
        GetAllOfficeLocationsQuery,
        (
            IEnumerable<OfficeLocation>,
            IEnumerable<AuthorizationConstants.Actions>,
            OfficeLocationCursor?
        )
    >
{
    private readonly IOfficeLocationRepository _officeLocationRepository;
    private readonly IAuthorizationService _authorizationService;

    private readonly IPaginationHelper _paginationHelper;

    /// <summary>
    /// Creates a new instance of <see cref="GetAllOfficeLocationsQueryHandler" />.
    /// </summary>
    public GetAllOfficeLocationsQueryHandler(
        IOfficeLocationRepository officeLocationRepository,
        IAuthorizationService authorizationService,
        IPaginationHelper paginationHelper
    )
    {
        _officeLocationRepository = officeLocationRepository;
        _authorizationService = authorizationService;
        _paginationHelper = paginationHelper;
    }

    /// <summary>
    /// Handles Query to return all Office Locations.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of Office Locations and allowed actions.</returns>
    public async Task<(
        IEnumerable<OfficeLocation>,
        IEnumerable<AuthorizationConstants.Actions>,
        OfficeLocationCursor?
    )> Handle(GetAllOfficeLocationsQuery request, CancellationToken cancellationToken)
    {
        var officeLocations = await _officeLocationRepository.GetOfficeLocationsAsync(
            request.Cursor
        );
        var (paginatedOfficeLocations, lastOfficeLocation) =
            await _paginationHelper.PaginateWithAuthAsync(officeLocations, request.Limit);
        var permissions = await _authorizationService.GetAllowedActions<OfficeLocation>(
            actions: [AuthorizationConstants.Actions.CREATE]
        );
        var nextCursor =
            lastOfficeLocation == null
                ? null
                : new OfficeLocationCursor(lastOfficeLocation.OfficeLocationName);
        return (paginatedOfficeLocations, permissions, nextCursor);
    }
}
