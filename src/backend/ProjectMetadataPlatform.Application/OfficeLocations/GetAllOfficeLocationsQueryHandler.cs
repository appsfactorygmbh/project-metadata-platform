using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Application.OfficeLocations;

/// <summary>
/// Handler for the <see cref="GetAllOfficeLocationsQuery" />.
/// </summary>
public class GetAllOfficeLocationsQueryHandler
    : IRequestHandler<GetAllOfficeLocationsQuery, IEnumerable<OfficeLocation>>
{
    private readonly IOfficeLocationRepository _officeLocationRepository;

    /// <summary>
    /// Creates a new instance of <see cref="GetAllOfficeLocationsQueryHandler" />.
    /// </summary>
    public GetAllOfficeLocationsQueryHandler(IOfficeLocationRepository officeLocationRepository)
    {
        _officeLocationRepository = officeLocationRepository;
    }

    /// <summary>
    /// Handles Query to return all Office Locations.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of Office Locations.</returns>
    public async Task<IEnumerable<OfficeLocation>> Handle(
        GetAllOfficeLocationsQuery request,
        CancellationToken cancellationToken
    )
    {
        var officeLocations = await _officeLocationRepository.GetOfficeLocationsAsync();
        return officeLocations.OrderBy(officeLocation =>
            officeLocation.OfficeLocationName.ToLowerInvariant()
        );
    }
}
