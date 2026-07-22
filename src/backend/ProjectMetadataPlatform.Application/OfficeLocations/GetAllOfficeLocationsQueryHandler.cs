using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
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
        (IEnumerable<OfficeLocation>, IEnumerable<AuthorizationConstants.Actions>)
    >
{
    private readonly IOfficeLocationRepository _officeLocationRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="GetAllOfficeLocationsQueryHandler" />.
    /// </summary>
    public GetAllOfficeLocationsQueryHandler(
        IOfficeLocationRepository officeLocationRepository,
        IAuthorizationService authorizationService
    )
    {
        _officeLocationRepository = officeLocationRepository;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles Query to return all Office Locations.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of Office Locations and allowed actions.</returns>
    public async Task<(
        IEnumerable<OfficeLocation>,
        IEnumerable<AuthorizationConstants.Actions>
    )> Handle(GetAllOfficeLocationsQuery request, CancellationToken cancellationToken)
    {
        var officeLocations = await _officeLocationRepository.GetOfficeLocationsAsync();
        var queriedOfficeLocations = await _authorizationService.TryGetPlanResourceQuery(
            officeLocations
        );
        var permissions = await _authorizationService.GetPermissions<OfficeLocation>(            actions: [AuthorizationConstants.Actions.CREATE]);
        if (queriedOfficeLocations == null)
        {
            List<OfficeLocation> filteredOfficeLocations = [];
            foreach (var officeLocation in officeLocations)
            {
                if (
                    await _authorizationService.CheckAccess(
                        officeLocation,
                        AuthorizationConstants.Actions.GET
                    )
                )
                {
                    filteredOfficeLocations.Add(officeLocation);
                }
            }
            return (
                filteredOfficeLocations.OrderBy(officeLocation =>
                    officeLocation.OfficeLocationName.ToLowerInvariant()
                ),
                permissions
            );
        }
        return (
            (
                await queriedOfficeLocations.ToListAsync(cancellationToken: cancellationToken)
            ).OrderBy(officeLocation => officeLocation.OfficeLocationName.ToLowerInvariant()),
            permissions
        );
    }
}
