using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.OfficeLocationExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Application.OfficeLocations;

/// <summary>
/// Handler for the <see cref=" UpdateOfficeLocationCommand" />.
/// </summary>
public class UpdateOfficeLocationCommandHandler
    : IRequestHandler<UpdateOfficeLocationCommand, OfficeLocation>
{
    private readonly IOfficeLocationRepository _officeLocationRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref=" UpdateOfficeLocationCommandHandler" />.
    /// </summary>
    public UpdateOfficeLocationCommandHandler(
        IOfficeLocationRepository officeLocationRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService
    )
    {
        _officeLocationRepository = officeLocationRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles Command to update an office location.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Updated Office Location.</returns>
    /// <exception cref="OfficeLocationNameAlreadyExistsException">When a office location with the new name already exists.</exception>
    public async Task<OfficeLocation> Handle(
        UpdateOfficeLocationCommand request,
        CancellationToken cancellationToken
    )
    {
        var location = await _officeLocationRepository.GetOfficeLocationAsync(request.Id);
        await CheckAuthorization(location, request);
        var logChanges = new List<LogChange> { };
        if (
            request.OfficeLocationName != null
            && request.OfficeLocationName != location.OfficeLocationName
        )
        {
            if (
                !string.Equals(
                    request.OfficeLocationName,
                    location.OfficeLocationName,
                    System.StringComparison.OrdinalIgnoreCase
                )
                && await _officeLocationRepository.CheckIfOfficeLocationNameExistsAsync(
                    request.OfficeLocationName
                )
            )
            {
                throw new OfficeLocationNameAlreadyExistsException(request.OfficeLocationName);
            }

            logChanges.Add(
                new LogChange
                {
                    Property = nameof(OfficeLocation.OfficeLocationName),
                    OldValue = location.OfficeLocationName,
                    NewValue = request.OfficeLocationName,
                }
            );
            location.OfficeLocationName = request.OfficeLocationName;
        }
        if (logChanges.Count > 0)
        {
            var updatedLocation = await _officeLocationRepository.UpdateOfficeLocationAsync(
                location
            );
            await _logRepository.AddOfficeLocationLogForCurrentActor(
                officeLocation: location,
                action: Action.UPDATED_OFFICE_LOCATION,
                logChanges
            );
            await _unitOfWork.CompleteAsync();
            return updatedLocation;
        }

        return location;
    }

    /// <summary>
    /// Checks Authorization for a Office Location and its update request.
    /// </summary>
    /// <param name="location">Requested Office Location.</param>
    /// <param name="request">Update Request for the Location.</param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException">Thrown if Update Request is unauthorized</exception>
    private async Task CheckAuthorization(
        OfficeLocation location,
        UpdateOfficeLocationCommand request
    )
    {
        Dictionary<string, object?> updates = [];
        if (request.OfficeLocationName != location.OfficeLocationName)
        {
            updates.Add(nameof(OfficeLocation.OfficeLocationName), request.OfficeLocationName);
        }
        if (
            !(
                await _authorizationService.CheckAccess(
                    location,
                    [AuthorizationConstants.Actions.EDIT],
                    updates
                )
            )[AuthorizationConstants.Actions.EDIT]
        )
        {
            throw new UnauthorizedException();
        }
    }
}
