using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.OfficeLocationExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Application.OfficeLocations;

public class CreateOfficeLocationCommandHandler : IRequestHandler<CreateOfficeLocationCommand, int>
{
    private readonly IOfficeLocationRepository _officeLocationRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOfficeLocationCommandHandler(
        IOfficeLocationRepository officeLocationRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork
    )
    {
        _officeLocationRepository = officeLocationRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(
        CreateOfficeLocationCommand request,
        CancellationToken cancellationToken
    )
    {
        if (
            await _officeLocationRepository.CheckIfOfficeLocationNameExistsAsync(
                request.OfficeLocationName
            )
        )
        {
            throw new OfficeLocationNameAlreadyExistsException(request.OfficeLocationName);
        }

        var officeLocation = new OfficeLocation { OfficeLocationName = request.OfficeLocationName };
        await AddOfficeLocationLog(officeLocation);
        await _officeLocationRepository.AddOfficeLocationAsync(officeLocation);
        await _unitOfWork.CompleteAsync();

        return officeLocation.Id;
    }

    private async Task AddOfficeLocationLog(OfficeLocation location)
    {
        var logChanges = new List<LogChange>
        {
            new LogChange
            {
                Property = nameof(OfficeLocation.OfficeLocationName),
                OldValue = "",
                NewValue = location.OfficeLocationName,
            },
        };

        await _logRepository.AddOfficeLocationLogForCurrentActor(
            location,
            Action.ADDED_OFFICE_LOCATION,
            logChanges
        );
    }
}
