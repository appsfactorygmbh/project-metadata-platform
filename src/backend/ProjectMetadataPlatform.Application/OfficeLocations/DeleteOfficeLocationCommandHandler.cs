using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.OfficeLocationExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Application.OfficeLocations;

public class DeleteOfficeLocationCommandHandler : IRequestHandler<DeleteOfficeLocationCommand>
{
    private readonly IOfficeLocationRepository _officeLocationRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteOfficeLocationCommandHandler(
        IOfficeLocationRepository officeLocationRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork
    )
    {
        _officeLocationRepository = officeLocationRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        DeleteOfficeLocationCommand request,
        CancellationToken cancellationToken
    )
    {
        var location = await _officeLocationRepository.GetOfficeLocationAsync(request.Id);
        _ = await _officeLocationRepository.DeleteOfficeLocationAsync(location);
        await _logRepository.AddOfficeLocationLogForCurrentActor(
            location,
            Action.REMOVED_OFFICE_LOCATION,
            []
        );
        await _unitOfWork.CompleteAsync();
    }
}
