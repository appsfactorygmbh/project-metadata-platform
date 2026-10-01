using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Billing;

namespace ProjectMetadataPlatform.Application.Billing;

/// <summary>
/// Handler for <see cref="GetAllBillingQuery"/>
/// </summary>
public class GetAllBillingQueryHandler
    : IRequestHandler<
        GetAllBillingQuery,
        (IEnumerable<GlobalBilling>, IEnumerable<AuthorizationConstants.Actions>, BillingCursor?)
    >
{
    private readonly IBillingRepository _billingRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly IPaginationHelper _paginationHelper;

    /// <summary>
    /// Creates new Instance of <see cref="GetAllBillingQueryHandler"/>
    /// </summary>
    /// <param name="billingRepository"></param>
    /// <param name="authorizationService"></param>
    /// <param name="paginationHelper"></param>
    public GetAllBillingQueryHandler(
        IBillingRepository billingRepository,
        IAuthorizationService authorizationService,
        IPaginationHelper paginationHelper
    )
    {
        _billingRepository = billingRepository;
        _authorizationService = authorizationService;
        _paginationHelper = paginationHelper;
    }

    /// <summary>
    /// Request to return all global billing objects.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<(
        IEnumerable<GlobalBilling>,
        IEnumerable<AuthorizationConstants.Actions>,
        BillingCursor?
    )> Handle(GetAllBillingQuery request, CancellationToken cancellationToken = default)
    {
        var billing = await _billingRepository.GetAllGlobalBillingInformationAsync(request.Cursor);

        var (paginatedBilling, lastBilling) = await _paginationHelper.PaginateWithAuthAsync(
            billing,
            request.Limit
        );
        var permissions = await _authorizationService.GetAllowedActions<GlobalBilling>(
            actions: [AuthorizationConstants.Actions.CREATE]
        );
        var nextCursor = lastBilling == null ? null : new BillingCursor(lastBilling.BillingKind);
        return (paginatedBilling, permissions, nextCursor);
    }
}
