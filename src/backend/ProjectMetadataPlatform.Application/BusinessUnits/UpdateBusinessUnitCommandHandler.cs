using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

public class UpdateBusinessUnitCommandHandler
    : IRequestHandler<UpdateBusinessUnitCommand, BusinessUnit>
{
    private readonly IBusinessUnitRepository _businessUnitRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

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
            businessUnit.BusinessUnitName = request.BusinessUnitName;
            logChanges.Add(
                new LogChange
                {
                    Property = nameof(BusinessUnit.BusinessUnitName),
                    OldValue = businessUnit.BusinessUnitName,
                    NewValue = request.BusinessUnitName,
                }
            );
        }
        if (logChanges.Count > 0)
        {
            var updatedBusinessUnit = await _businessUnitRepository.UpdateBusinessUnitAsync(
                businessUnit
            );
            await _logRepository.AddBusinessUnitLogForCurrentActor(
                bu: businessUnit,
                action: Action.UPDATED_DEPARTMENT,
                logChanges
            );
            await _unitOfWork.CompleteAsync();
            return updatedBusinessUnit;
        }

        return businessUnit;
    }
}
