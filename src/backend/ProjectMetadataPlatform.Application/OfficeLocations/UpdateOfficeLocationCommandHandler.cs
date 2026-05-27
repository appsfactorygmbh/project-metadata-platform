using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.OfficeLocationExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Application.OfficeLocations;

public class UpdateOfficeLocationCommandHandler
    : IRequestHandler<UpdateOfficeLocationCommand, OfficeLocation>
{
    private readonly IOfficeLocationRepository _officeLocationRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

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
            location.OfficeLocationName = request.OfficeLocationName;
            logChanges.Add(
                new LogChange
                {
                    Property = nameof(OfficeLocation.OfficeLocationName),
                    OldValue = location.OfficeLocationName,
                    NewValue = request.OfficeLocationName,
                }
            );
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
