using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.BusinessUnits;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

/// <summary>
/// Handler for the <see cref="GetAllBusinessUnitsQuery" />.
/// </summary>
public class GetAllBusinessUnitsQueryHandler
    : IRequestHandler<
        GetAllBusinessUnitsQuery,
        (IEnumerable<BusinessUnit>, IEnumerable<AuthorizationConstants.Actions>)
    >
{
    private readonly IBusinessUnitRepository _businessUnitRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="GetAllBusinessUnitsQueryHandler" />.
    /// </summary>
    public GetAllBusinessUnitsQueryHandler(
        IBusinessUnitRepository businessUnitRepository,
        IAuthorizationService authorizationService
    )
    {
        _businessUnitRepository = businessUnitRepository;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles a Request to return all bu's.
    /// </summary>
    /// <param name="request">Request that is handled.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of BU's and allowed actions</returns>
    public async Task<(
        IEnumerable<BusinessUnit>,
        IEnumerable<AuthorizationConstants.Actions>
    )> Handle(GetAllBusinessUnitsQuery request, CancellationToken cancellationToken)
    {
        var businessUnits = await _businessUnitRepository.GetBusinessUnitsAsync();
        var queriedBusinessUnits = await _authorizationService.TryGetPlanResourceQuery(
            businessUnits
        );
        var permissions = await _authorizationService.GetPermissions<BusinessUnit>(            actions: [AuthorizationConstants.Actions.CREATE]);
        if (queriedBusinessUnits == null)
        {
            List<BusinessUnit> filteredBusinessUnits = [];
            foreach (var businessUnit in businessUnits)
            {
                if (
                    await _authorizationService.CheckAccess(
                        businessUnit,
                        AuthorizationConstants.Actions.GET
                    )
                )
                {
                    filteredBusinessUnits.Add(businessUnit);
                }
            }
            return (
                filteredBusinessUnits.OrderBy(businessUnit =>
                    businessUnit.BusinessUnitName.ToLowerInvariant()
                ),
                permissions
            );
        }
        return (
            (await queriedBusinessUnits.ToListAsync(cancellationToken: cancellationToken)).OrderBy(
                businessUnit => businessUnit.BusinessUnitName.ToLowerInvariant()
            ),
            permissions
        );
    }
}
