using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Application.OfficeLocations;

/// <summary>
/// Handler for the <see cref="GetOfficeLocationQuery" />.
/// </summary>
public class GetOfficeLocationQueryHandler : IRequestHandler<GetOfficeLocationQuery, OfficeLocation>
{
    private readonly IOfficeLocationRepository _officeLocationRepository;

    /// <summary>
    /// Creates a new instance of <see cref="GetOfficeLocationQueryHandler" />.
    /// </summary>
    public GetOfficeLocationQueryHandler(IOfficeLocationRepository officeLocationRepository)
    {
        _officeLocationRepository = officeLocationRepository;
    }

    /// <summary>
    /// Handles a Query to return an office location by id.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Office Location.</returns>
    public async Task<OfficeLocation> Handle(
        GetOfficeLocationQuery request,
        CancellationToken cancellationToken
    )
    {
        return await _officeLocationRepository.GetOfficeLocationAsync(request.Id);
    }
}
