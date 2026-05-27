using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Application.OfficeLocations;

public class GetAllOfficeLocationsQueryHandler
    : IRequestHandler<GetAllOfficeLocationsQuery, IEnumerable<OfficeLocation>>
{
    private readonly IOfficeLocationRepository _officeLocationRepository;

    public GetAllOfficeLocationsQueryHandler(IOfficeLocationRepository officeLocationRepository)
    {
        _officeLocationRepository = officeLocationRepository;
    }

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
