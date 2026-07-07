using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;

namespace ProjectMetadataPlatform.Application.BusinessUnits;

/// <summary>
/// Handler for the <see cref="GetBusinessUnitQuery" />.
/// </summary>
public class GetBusinessUnitQueryHandler : IRequestHandler<GetBusinessUnitQuery, BusinessUnit>
{
    private readonly IBusinessUnitRepository _businessUnitRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="GetBusinessUnitQueryHandler" />.
    /// </summary>
    public GetBusinessUnitQueryHandler(
        IBusinessUnitRepository businessUnitRepository,
        IAuthorizationService authorizationService
    )
    {
        _businessUnitRepository = businessUnitRepository;
        _authorizationService = authorizationService;
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
        var bu = await _businessUnitRepository.GetBusinessUnitAsync(request.Id);
        if (
            !(await _authorizationService.CheckAccess(bu, [AuthorizationConstants.Actions.GET]))[
                AuthorizationConstants.Actions.GET
            ]
        )
        {
            throw new UnauthorizedException();
        }
        return bu;
    }
}
