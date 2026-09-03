using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Billing;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;

namespace ProjectMetadataPlatform.Application.Billing;

/// <summary>
/// Handler for <see cref="GetBillingByIdQuery"/>
/// </summary>
public class GetBillingByIdQueryHandler
    : IRequestHandler<
        GetBillingByIdQuery,
        (GlobalBilling, IEnumerable<AuthorizationConstants.Actions>)
    >
{
    private readonly IBillingRepository _billingRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new Instance of <see cref="GetBillingByIdQueryHandler"/>
    /// </summary>
    /// <param name="billingRepository"></param>
    /// <param name="authorizationService"></param>
    public GetBillingByIdQueryHandler(
        IBillingRepository billingRepository,
        IAuthorizationService authorizationService
    )
    {
        _billingRepository = billingRepository;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles Request to return billing information by id.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    public async Task<(GlobalBilling, IEnumerable<AuthorizationConstants.Actions>)> Handle(
        GetBillingByIdQuery request,
        CancellationToken cancellationToken = default
    )
    {
        var billing = await _billingRepository.GetBillingByIdAsync(request.Id);
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
