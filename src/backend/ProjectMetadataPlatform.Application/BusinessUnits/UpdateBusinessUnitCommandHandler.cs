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
/// Handler for the <see cref="UpdateBusinessUnitCommand" />.
/// </summary>
public class UpdateBusinessUnitCommandHandler
    : IRequestHandler<UpdateBusinessUnitCommand, BusinessUnit>
{
    private readonly IBusinessUnitRepository _businessUnitRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Creates a new instance of <see cref="UpdateBusinessUnitCommandHandler" />.
    /// </summary>
    public UpdateBusinessUnitCommandHandler(
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
    /// Handles Command to update a BU.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Updated BU.</returns>
    /// <exception cref="BusinessUnitNameAlreadyExistsException">Thrown if updated Name already exists.</exception>
    public async Task<BusinessUnit> Handle(
        UpdateBusinessUnitCommand request,
        CancellationToken cancellationToken
    )
    {
        var businessUnit = await _businessUnitRepository.GetBusinessUnitAsync(request.Id);
        var logChanges = new List<LogChange> { };
        if (
            request.BusinessUnitName != null
            && request.BusinessUnitName != businessUnit.BusinessUnitName
        )
        {
            if (
                !string.Equals(
                    request.BusinessUnitName,
                    businessUnit.BusinessUnitName,
                    System.StringComparison.OrdinalIgnoreCase
                )
                && await _businessUnitRepository.CheckIfBusinessUnitNameExistsAsync(
                    request.BusinessUnitName
                )
            )
            {
                throw new BusinessUnitNameAlreadyExistsException(request.BusinessUnitName);
            }

            logChanges.Add(
                new LogChange
                {
                    Property = nameof(BusinessUnit.BusinessUnitName),
                    OldValue = businessUnit.BusinessUnitName,
                    NewValue = request.BusinessUnitName,
                }
            );
            businessUnit.BusinessUnitName = request.BusinessUnitName;
        }
        if (logChanges.Count > 0)
        {
            var updatedBusinessUnit = await _businessUnitRepository.UpdateBusinessUnitAsync(
                businessUnit
            );
            await _logRepository.AddBusinessUnitLogForCurrentActor(
                bu: businessUnit,
                action: Action.UPDATED_BUSINESS_UNIT,
                logChanges
            );
            await _unitOfWork.CompleteAsync();
            return updatedBusinessUnit;
        }

        return businessUnit;
    }
}
