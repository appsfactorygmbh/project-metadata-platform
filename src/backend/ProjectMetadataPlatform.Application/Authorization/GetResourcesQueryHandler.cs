using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Authorization;

/// <summary>
/// Handler for <see cref="GetResourcesQuery"/>
/// </summary>
public class GetResourcesQueryHandler : IRequestHandler<GetResourcesQuery, IEnumerable<string>>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IAuthorizationAdminService _authorizationAdminService;

    /// <summary>
    /// Creates an Instance of <see cref="GetResourcesQueryHandler"/>
    /// </summary>
    public GetResourcesQueryHandler(
        IAuthorizationAdminService authorizationAdminService,
        IAuthorizationService authorizationService
    )
    {
        _authorizationAdminService = authorizationAdminService;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handling for Returning all Resource types.
    /// </summary>
    /// <param name="request">Request to be handled.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of names.</returns>
    public async Task<IEnumerable<string>> Handle(
        GetResourcesQuery request,
        CancellationToken cancellationToken = default
    )
    {
        // allowed for every authenticated user.
        await _authorizationService.BypassAuthorization();
        return await _authorizationAdminService.GetResources();
    }
}
