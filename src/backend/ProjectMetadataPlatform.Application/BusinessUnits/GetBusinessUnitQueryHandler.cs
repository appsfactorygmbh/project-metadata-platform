using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.BusinessUnits;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

public class GetBusinessUnitQueryHandler : IRequestHandler<GetBusinessUnitQuery, BusinessUnit>
{
    private readonly IBusinessUnitRepository _businessUnitRepository;

    public GetBusinessUnitQueryHandler(IBusinessUnitRepository businessUnitRepository)
    {
        _businessUnitRepository = businessUnitRepository;
    }

    public async Task<BusinessUnit> Handle(
        GetBusinessUnitQuery request,
        CancellationToken cancellationToken
    )
    {
        return await _businessUnitRepository.GetBusinessUnitAsync(request.Id);
    }
}
