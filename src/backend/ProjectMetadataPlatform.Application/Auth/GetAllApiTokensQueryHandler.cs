using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Handler for the <see cref="GetAllApiTokensQuery" />
/// </summary>
public class GetAllApiTokensQueryHandler
    : IRequestHandler<GetAllApiTokensQuery, IEnumerable<ApiToken>>
{
    private readonly IApiTokenRepository _apiTokenRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="GetAllApiTokensQueryHandler" />.
    /// </summary>
    /// <param name="apiTokenRepository"></param>
    /// <param name="authorizationService"></param>
    public GetAllApiTokensQueryHandler(
        IApiTokenRepository apiTokenRepository,
        IAuthorizationService authorizationService
    )
    {
        _apiTokenRepository = apiTokenRepository;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Gets a List of all Api Tokens.
    /// </summary>
    /// <param name="request">Request that is handled.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of Api Tokens</returns>
    public async Task<IEnumerable<ApiToken>> Handle(
        GetAllApiTokensQuery request,
        CancellationToken cancellationToken
    )
    {
        var tokens = await _apiTokenRepository.GetApiTokens();

        var queriedTokens = await _authorizationService.TryGetPlanResourceQuery(tokens);
        if (queriedTokens == null)
        {
            List<ApiToken> apiTokens = [];
            foreach (var token in tokens)
            {
                if (
                    (
                        await _authorizationService.CheckAccess(
                            token,
                            [AuthorizationConstants.Actions.GET]
                        )
                    )[AuthorizationConstants.Actions.GET]
                )
                {
                    apiTokens.Add(token);
                }
            }
            return apiTokens;
        }
        else
        {
            return await queriedTokens.ToListAsync(cancellationToken);
        }
    }
}
