using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.BusinessUnits;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

public class GetAllBusinessUnitsQueryHandler
    : IRequestHandler<GetAllBusinessUnitsQuery, IEnumerable<BusinessUnit>>
{
    private readonly IBusinessUnitRepository _businessUnitRepository;

    public GetAllBusinessUnitsQueryHandler(IBusinessUnitRepository businessUnitRepository)
    {
        _businessUnitRepository = businessUnitRepository;
    }

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
