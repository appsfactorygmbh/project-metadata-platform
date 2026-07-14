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
/// Handler for the <see cref="CreateOfficeLocationCommand" />.
/// </summary>
public class CreateOfficeLocationCommandHandler : IRequestHandler<CreateOfficeLocationCommand, int>
{
    private readonly IOfficeLocationRepository _officeLocationRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="CreateOfficeLocationCommandHandler" />.
    /// </summary>
    public CreateOfficeLocationCommandHandler(
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
    /// Handles a Command to create a new office location.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Id of the new office location.</returns>
    /// <exception cref="OfficeLocationNameAlreadyExistsException">Thrown if a office location with the name in the command already exists.</exception>
    public async Task<int> Handle(
        CreateOfficeLocationCommand request,
        CancellationToken cancellationToken
    )
    {
        var officeLocation = new OfficeLocation { OfficeLocationName = request.OfficeLocationName };
        if (
            !await _authorizationService.CheckAccess(
                officeLocation,
                AuthorizationConstants.Actions.CREATE
            )
        )
        {
            throw new UnauthorizedException();
        }
        if (
            await _officeLocationRepository.CheckIfOfficeLocationNameExistsAsync(
                request.OfficeLocationName
            )
        )
        {
            throw new OfficeLocationNameAlreadyExistsException(request.OfficeLocationName);
        }
        await AddOfficeLocationLog(officeLocation);
        await _officeLocationRepository.AddOfficeLocationAsync(officeLocation);
        await _unitOfWork.CompleteAsync();

        return officeLocation.Id;
    }

    /// <summary>
    /// Adds a log entry for the new location.
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
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
