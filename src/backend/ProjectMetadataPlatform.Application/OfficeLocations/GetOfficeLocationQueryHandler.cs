using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Application.OfficeLocations;

public class GetOfficeLocationQueryHandler : IRequestHandler<GetOfficeLocationQuery, OfficeLocation>
{
    private readonly IOfficeLocationRepository _officeLocationRepository;

    public GetOfficeLocationQueryHandler(IOfficeLocationRepository officeLocationRepository)
    {
        _officeLocationRepository = officeLocationRepository;
    }

    public async Task<OfficeLocation> Handle(
        GetOfficeLocationQuery request,
        CancellationToken cancellationToken
    )
    {
        return await _officeLocationRepository.GetOfficeLocationAsync(request.id);
    }
}
