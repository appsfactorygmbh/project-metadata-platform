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
public class GetOfficeLocationQueryHandler : IRequestHandler<GetOfficeLocationQuery, OfficeLocation>
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
    /// <returns>A Office Location.</returns>
    public async Task<OfficeLocation> Handle(
        GetOfficeLocationQuery request,
        CancellationToken cancellationToken
    )
    {
        var location = await _officeLocationRepository.GetOfficeLocationAsync(request.Id);
        if (
            !(
                await _authorizationService.CheckAccess(
                    location,
                    [AuthorizationConstants.Actions.GET]
                )
            )[AuthorizationConstants.Actions.GET]
        )
        {
            throw new UnauthorizedException();
        }
        return location;
    }
}
