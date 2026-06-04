using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
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

    /// <summary>
    /// Creates a new instance of <see cref=" UpdateOfficeLocationCommandHandler" />.
    /// </summary>
    public UpdateOfficeLocationCommandHandler(
        IOfficeLocationRepository officeLocationRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork
    )
    {
        _officeLocationRepository = officeLocationRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
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
}
