using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

/// <summary>
/// Handler for the <see cref="DeleteBusinessUnitCommand" />.
/// </summary>
public class DeleteBusinessUnitCommandHandler : IRequestHandler<DeleteBusinessUnitCommand>
{
    private readonly IBusinessUnitRepository _businessUnitRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="DeleteBusinessUnitCommandHandler" />.
    /// </summary>
    public DeleteBusinessUnitCommandHandler(
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
    /// Handles a Deletion request for a BU.
    /// </summary>
    /// <param name="request">Request to be handled.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="BusinessUnitStillLinkedToTeamsException">Thrown if the bu is still linked to a team.</exception>
    public async Task Handle(DeleteBusinessUnitCommand request, CancellationToken cancellationToken)
    {
        var businessUnit = await _businessUnitRepository.GetBusinessUnitWithTeamsAsync(request.Id);
        if (
            !await _authorizationService.CheckAccess(
                businessUnit,
                AuthorizationConstants.Actions.DELETE
            )
        )
        {
            throw new UnauthorizedException();
        }
        if (businessUnit.Teams != null && businessUnit.Teams.Count > 0)
        {
            throw new BusinessUnitStillLinkedToTeamsException(
                businessUnit,
                [.. businessUnit.Teams.Select(team => team.Id)]
            );
        }
        _ = await _businessUnitRepository.DeleteBusinessUnitAsync(businessUnit);
        await _logRepository.AddBusinessUnitLogForCurrentActor(
            businessUnit,
            Action.REMOVED_BUSINESS_UNIT,
            []
        );
        await _unitOfWork.CompleteAsync();
    }
}
