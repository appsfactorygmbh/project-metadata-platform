using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

/// <summary>
/// Handler for the <see cref="GetLinkedTeamsQuery" />.
/// </summary>
public class GetLinkedTeamsQueryHandler : IRequestHandler<GetLinkedTeamsQuery, List<int>>
{
    private readonly IBusinessUnitRepository _businessUnitRepository;

    /// <summary>
    /// Creates a new instance of <see cref="GetLinkedTeamsQueryHandler" />.
    /// </summary>
    public GetLinkedTeamsQueryHandler(IBusinessUnitRepository businessUnitRepository)
    {
        _businessUnitRepository = businessUnitRepository;
    }

    /// <summary>
    /// Handles the request to return linked teams.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of Team Id's.</returns>
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
