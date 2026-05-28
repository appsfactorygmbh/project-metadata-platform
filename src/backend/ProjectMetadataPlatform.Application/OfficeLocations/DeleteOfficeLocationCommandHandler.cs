using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.OfficeLocations;

/// <summary>
/// Handler for the <see cref="DeleteOfficeLocationCommand" />.
/// </summary>
public class DeleteOfficeLocationCommandHandler : IRequestHandler<DeleteOfficeLocationCommand>
{
    private readonly IOfficeLocationRepository _officeLocationRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Creates a new instance of <see cref="DeleteOfficeLocationCommandHandler" />.
    /// </summary>
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

    /// <summary>
    /// Handles Command to delete an Office Location.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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
