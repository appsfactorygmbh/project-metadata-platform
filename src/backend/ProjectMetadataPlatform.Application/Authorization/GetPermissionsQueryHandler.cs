using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Application.Authorization;

/// <summary>
/// Handler for <see cref="GetPermissionsQuery"/>
/// </summary>
public class GetPermissionsQueryHandler
    : IRequestHandler<GetPermissionsQuery, Dictionary<AuthorizationConstants.Actions, string>>
{
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new Instance of <see cref="GetPermissionsQueryHandler"/>
    /// </summary>
    public GetPermissionsQueryHandler(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handling for Returning all Permissions for a Resource type.
    /// </summary>
    /// <param name="request">Request to be handled.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Dictionary of Actions with their permission filter.</returns>
    public async Task<Dictionary<AuthorizationConstants.Actions, string>> Handle(
        GetPermissionsQuery request,
        CancellationToken cancellationToken = default
    )
    {
        // allowed for every authenticated user.
        await _authorizationService.BypassAuthorization();
        return await _authorizationService.GetPermissions(request.ResourceKind);
    }
}
