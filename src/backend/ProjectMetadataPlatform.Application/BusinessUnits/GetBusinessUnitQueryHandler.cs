using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.BusinessUnits;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

/// <summary>
/// Handler for the <see cref="GetBusinessUnitQuery" />.
/// </summary>
public class GetBusinessUnitQueryHandler : IRequestHandler<GetBusinessUnitQuery, BusinessUnit>
{
    private readonly IBusinessUnitRepository _businessUnitRepository;

    /// <summary>
    /// Creates a new instance of <see cref="GetBusinessUnitQueryHandler" />.
    /// </summary>
    public GetBusinessUnitQueryHandler(IBusinessUnitRepository businessUnitRepository)
    {
        _businessUnitRepository = businessUnitRepository;
    }

    /// <summary>
    /// Handles the Request to return a specific Business Unit.
    /// </summary>
    /// <param name="request">Request that is handled.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A Business Unit.</returns>
    public async Task<BusinessUnit> Handle(
        GetBusinessUnitQuery request,
        CancellationToken cancellationToken
    )
    {
        return await _businessUnitRepository.GetBusinessUnitAsync(request.Id);
    }
}
