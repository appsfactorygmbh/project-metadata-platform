using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
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
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="UpdateBusinessUnitCommandHandler" />.
    /// </summary>
    public UpdateBusinessUnitCommandHandler(
        IBusinessUnitRepository businessUnitRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService
    )
    {
        _businessUnitRepository = businessUnitRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
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
        await CheckAuthorization(businessUnit, request);
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

    /// <summary>
    /// Checks Authorization for a Business Unit and its update request.
    /// </summary>
    /// <param name="businessUnit">Business Unit that is requested to be updated.</param>
    /// <param name="request">Update Request for the Business Unit</param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException">Thrown if Update Request is unauthorized</exception>
    private async Task CheckAuthorization(
        BusinessUnit businessUnit,
        UpdateBusinessUnitCommand request
    )
    {
        Dictionary<string, object?> updates = [];
        if (request.BusinessUnitName != businessUnit.BusinessUnitName)
        {
            updates.Add(nameof(BusinessUnit.BusinessUnitName), request.BusinessUnitName);
        }
        if (
            !(
                await _authorizationService.CheckAccess(
                    businessUnit,
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
