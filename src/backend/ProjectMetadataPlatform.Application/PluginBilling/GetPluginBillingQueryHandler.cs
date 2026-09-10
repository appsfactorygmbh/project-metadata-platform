using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;

namespace ProjectMetadataPlatform.Application.PluginBilling;

/// <summary>
/// Handler for <see cref="GetPluginBillingQuery"/>
/// </summary>
public class GetPluginBillingQueryHandler
    : IRequestHandler<
        GetPluginBillingQuery,
        (Domain.Billing.PluginBilling, IEnumerable<AuthorizationConstants.Actions>)
    >
{
    private readonly IBillingRepository _billingRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates new Instance of <see cref="GetPluginBillingQueryHandler"/>
    /// </summary>
    /// <param name="billingRepository"></param>
    /// <param name="authorizationService"></param>
    public GetPluginBillingQueryHandler(
        IBillingRepository billingRepository,
        IAuthorizationService authorizationService
    )
    {
        _billingRepository = billingRepository;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles request to return billing for a plugin.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    public async Task<(
        Domain.Billing.PluginBilling,
        IEnumerable<AuthorizationConstants.Actions>
    )> Handle(GetPluginBillingQuery request, CancellationToken cancellationToken = default)
    {
        var billing = await _billingRepository.GetPluginBillingByIdAsync(
            request.ProjectId,
            request.PluginId
        );
        if (!await _authorizationService.CheckAccess(billing, AuthorizationConstants.Actions.GET))
        {
            throw new UnauthorizedException();
        }
        var permissions = await _authorizationService.GetAllowedActions(
            billing,
            [AuthorizationConstants.Actions.EDIT, AuthorizationConstants.Actions.DELETE]
        );
        return (billing, permissions);
    }
}
