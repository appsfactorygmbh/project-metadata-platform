using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
        (IEnumerable<GlobalBilling>, IEnumerable<AuthorizationConstants.Actions>)
    >
{
    private readonly IBillingRepository _billingRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates new Instance of <see cref="GetAllBillingQueryHandler"/>
    /// </summary>
    /// <param name="billingRepository"></param>
    /// <param name="authorizationService"></param>
    public GetAllBillingQueryHandler(
        IBillingRepository billingRepository,
        IAuthorizationService authorizationService
    )
    {
        _billingRepository = billingRepository;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Request to return all global billing objects.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<(
        IEnumerable<GlobalBilling>,
        IEnumerable<AuthorizationConstants.Actions>
    )> Handle(GetAllBillingQuery request, CancellationToken cancellationToken = default)
    {
        var billing = await _billingRepository.GetAllGlobalBillingInformationAsync();

        var queriedBilling = await _authorizationService.TryGetPlanResourceQuery(billing);
        var permissions = await _authorizationService.GetAllowedActions<GlobalBilling>(
            actions: [AuthorizationConstants.Actions.CREATE]
        );
        if (queriedBilling == null)
        {
            var billingList = await billing.ToListAsync(cancellationToken: cancellationToken);
            List<GlobalBilling> billingInformation = [];
            foreach (var billingObject in billingList)
            {
                if (
                    await _authorizationService.CheckAccess(
                        billingObject,
                        AuthorizationConstants.Actions.GET
                    )
                )
                {
                    billingInformation.Add(billingObject);
                }
            }
            return (billingInformation, permissions);
        }
        else
        {
            return (await queriedBilling.ToListAsync(cancellationToken), permissions);
        }
    }
}
