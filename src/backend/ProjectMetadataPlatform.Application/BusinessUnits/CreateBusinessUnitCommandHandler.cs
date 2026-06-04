using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

/// <summary>
/// Handler for the <see cref="CreateBusinessUnitCommand" />.
/// </summary>
public class CreateBusinessUnitCommandHandler : IRequestHandler<CreateBusinessUnitCommand, int>
{
    private readonly IBusinessUnitRepository _businessUnitRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Creates a new instance of <see cref="CreateBusinessUnitCommandHandler " />.
    /// </summary>
    public CreateBusinessUnitCommandHandler(
        IBusinessUnitRepository businessUnitRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork
    )
    {
        _businessUnitRepository = businessUnitRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the request to create a new bu.
    /// </summary>
    /// <param name="request">Request to be handled</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Id of new BU</returns>
    /// <exception cref="BusinessUnitNameAlreadyExistsException">When Bu with the same name already exists.</exception>
    public async Task<int> Handle(
        CreateBusinessUnitCommand request,
        CancellationToken cancellationToken
    )
    {
        if (
            await _businessUnitRepository.CheckIfBusinessUnitNameExistsAsync(
                request.BusinessUnitName
            )
        )
        {
            throw new BusinessUnitNameAlreadyExistsException(request.BusinessUnitName);
        }

        var bu = new BusinessUnit { BusinessUnitName = request.BusinessUnitName };
        await AddBusinessUnitLog(bu);
        await _businessUnitRepository.AddBusinessUnitAsync(bu);
        await _unitOfWork.CompleteAsync();

        return bu.Id;
    }

    /// <summary>
    /// Adds a Log Entry for Creating a Bu.
    /// </summary>
    /// <param name="bu">Newly Created BU</param>
    /// <returns></returns>
    private async Task AddBusinessUnitLog(BusinessUnit bu)
    {
        var logChanges = new List<LogChange>
        {
            new LogChange
            {
                Property = nameof(BusinessUnit.BusinessUnitName),
                OldValue = "",
                NewValue = bu.BusinessUnitName,
            },
        };

        await _logRepository.AddBusinessUnitLogForCurrentActor(
            bu,
            Action.ADDED_BUSINESS_UNIT,
            logChanges
        );
    }
}
