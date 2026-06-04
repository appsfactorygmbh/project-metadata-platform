using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.BusinessUnits;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

/// <summary>
/// Handler for the <see cref="GetAllBusinessUnitsQuery" />.
/// </summary>
public class GetAllBusinessUnitsQueryHandler
    : IRequestHandler<GetAllBusinessUnitsQuery, IEnumerable<BusinessUnit>>
{
    private readonly IBusinessUnitRepository _businessUnitRepository;

    /// <summary>
    /// Creates a new instance of <see cref="GetAllBusinessUnitsQueryHandler" />.
    /// </summary>
    public GetAllBusinessUnitsQueryHandler(IBusinessUnitRepository businessUnitRepository)
    {
        _businessUnitRepository = businessUnitRepository;
    }

    /// <summary>
    /// Handles a Request to return all bu's.
    /// </summary>
    /// <param name="request">Request that is handled.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of BU's</returns>
    public async Task<IEnumerable<BusinessUnit>> Handle(
        GetAllBusinessUnitsQuery request,
        CancellationToken cancellationToken
    )
    {
        var businessUnits = await _businessUnitRepository.GetBusinessUnitsAsync();
        return businessUnits.OrderBy(businessUnit =>
            businessUnit.BusinessUnitName.ToLowerInvariant()
        );
    }
}
