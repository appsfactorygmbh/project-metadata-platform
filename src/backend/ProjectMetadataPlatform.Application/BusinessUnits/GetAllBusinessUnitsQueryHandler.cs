using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        (
            IEnumerable<BusinessUnit>,
            IEnumerable<AuthorizationConstants.Actions>,
            BusinessUnitCursor?
        )
    >
{
    private readonly IBusinessUnitRepository _businessUnitRepository;
    private readonly IAuthorizationService _authorizationService;

    private readonly IPaginationHelper _paginationHelper;

    /// <summary>
    /// Creates a new instance of <see cref="GetAllBusinessUnitsQueryHandler" />.
    /// </summary>
    public GetAllBusinessUnitsQueryHandler(
        IBusinessUnitRepository businessUnitRepository,
        IAuthorizationService authorizationService,
        IPaginationHelper paginationHelper
    )
    {
        _businessUnitRepository = businessUnitRepository;
        _authorizationService = authorizationService;
        _paginationHelper = paginationHelper;
    }

    /// <summary>
    /// Handles a Request to return all bu's.
    /// </summary>
    /// <param name="request">Request that is handled.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of BU's and allowed actions</returns>
    public async Task<(
        IEnumerable<BusinessUnit>,
        IEnumerable<AuthorizationConstants.Actions>,
        BusinessUnitCursor?
    )> Handle(GetAllBusinessUnitsQuery request, CancellationToken cancellationToken)
    {
        var businessUnits = await _businessUnitRepository.GetBusinessUnitsAsync(request.Cursor);
        var (paginatedBusinessUnits, lastBusinessUnit) =
            await _paginationHelper.PaginateWithAuthAsync(businessUnits, request.Limit);
        var permissions = await _authorizationService.GetAllowedActions<BusinessUnit>(
            actions: [AuthorizationConstants.Actions.CREATE]
        );
        var nextCursor =
            lastBusinessUnit == null
                ? null
                : new BusinessUnitCursor(lastBusinessUnit.BusinessUnitName);
        return (paginatedBusinessUnits, permissions, nextCursor);
    }
}
