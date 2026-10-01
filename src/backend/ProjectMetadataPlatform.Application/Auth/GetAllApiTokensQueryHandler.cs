using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Handler for the <see cref="GetAllApiTokensQuery" />
/// </summary>
public class GetAllApiTokensQueryHandler
    : IRequestHandler<
        GetAllApiTokensQuery,
        (IEnumerable<ApiToken>, IEnumerable<AuthorizationConstants.Actions>, ApiTokenCursor?)
    >
{
    private readonly IApiTokenRepository _apiTokenRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly IPaginationHelper _paginationHelper;

    /// <summary>
    /// Creates a new instance of <see cref="GetAllApiTokensQueryHandler" />.
    /// </summary>
    /// <param name="apiTokenRepository"></param>
    /// <param name="authorizationService"></param>
    /// <param name="paginationHelper"></param>
    public GetAllApiTokensQueryHandler(
        IApiTokenRepository apiTokenRepository,
        IAuthorizationService authorizationService,
        IPaginationHelper paginationHelper
    )
    {
        _apiTokenRepository = apiTokenRepository;
        _authorizationService = authorizationService;
        _paginationHelper = paginationHelper;
    }

    /// <summary>
    /// Gets a List of all Api Tokens.
    /// </summary>
    /// <param name="request">Request that is handled.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of Api Tokens and allowed actions</returns>
    public async Task<(
        IEnumerable<ApiToken>,
        IEnumerable<AuthorizationConstants.Actions>,
        ApiTokenCursor?
    )> Handle(GetAllApiTokensQuery request, CancellationToken cancellationToken)
    {
        var tokens = await _apiTokenRepository.GetApiTokens(request.Cursor);
        var (paginatedTokens, lastToken) = await _paginationHelper.PaginateWithAuthAsync(
            tokens,
            request.Limit
        );
        var permissions = await _authorizationService.GetAllowedActions<ApiToken>(
            actions: [AuthorizationConstants.Actions.CREATE]
        );
        var nextCursor = lastToken == null ? null : new ApiTokenCursor(lastToken.Name);
        return (paginatedTokens, permissions, nextCursor);
    }
}
