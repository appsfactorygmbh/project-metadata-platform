using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.BusinessUnits;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

public class GetLinkedTeamsQueryHandler : IRequestHandler<GetLinkedTeamsQuery, List<int>>
{
    private readonly IBusinessUnitRepository _businessUnitRepository;

    public GetLinkedTeamsQueryHandler(IBusinessUnitRepository businessUnitRepository)
    {
        _businessUnitRepository = businessUnitRepository;
    }

    public async Task<List<int>> Handle(
        GetLinkedTeamsQuery request,
        CancellationToken cancellationToken
    )
    {
        return
        [
            .. (
                (await _businessUnitRepository.GetBusinessUnitWithTeamsAsync(request.Id)).Teams
                ?? []
            ).Select(team => team.Id),
        ];
    }
}
