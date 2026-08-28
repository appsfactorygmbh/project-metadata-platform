using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.PluginBilling;

/// <summary>
/// Handler for <see cref="DeletePluginBillingCommand"/>
/// </summary>
public class DeletePluginBillingCommandHandler : IRequestHandler<DeletePluginBillingCommand>
{
    private readonly IBillingRepository _billingRepository;

    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates new Instance of <see cref="DeletePluginBillingCommandHandler"/>
    /// </summary>
    /// <param name="billingRepository"></param>
    /// <param name="logRepository"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="authorizationService"></param>
    public DeletePluginBillingCommandHandler(
        IBillingRepository billingRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService
    )
    {
        _billingRepository = billingRepository;

        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles request to delete billing information.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    public async Task Handle(
        DeletePluginBillingCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var billing = await _billingRepository.GetPluginBillingByIdAsync(
            request.ProjectId,
            request.PluginId
        );

        if (
            !await _authorizationService.CheckAccess(billing, AuthorizationConstants.Actions.DELETE)
        )
        {
            throw new UnauthorizedException();
        }

        await _billingRepository.DeletePluginBillingAsync(billing);

        await _logRepository.AddProjectLogForCurrentActor(
            billing.ProjectPlugin?.Project!,
            Action.REMOVED_PROJECT_PLUGIN_BILLING,
            []
        );
        await _unitOfWork.CompleteAsync();
    }
}
