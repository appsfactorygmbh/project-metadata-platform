using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Errors.AuthExceptions;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Handler for the <see cref="GetAllApiTokensQuery" />
/// </summary>
public class GetAllApiTokensQueryHandler
    : IRequestHandler<GetAllApiTokensQuery, IEnumerable<ApiToken>>
{
    private readonly IApiTokenRepository _apiTokenRepository;

    /// <summary>
    /// Creates a new instance of <see cref="GetAllApiTokensQueryHandler" />.
    /// </summary>
    /// <param name="apiTokenRepository"></param>
    public GetAllApiTokensQueryHandler(IApiTokenRepository apiTokenRepository)
    {
        _apiTokenRepository = apiTokenRepository;
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

        return tokens;
    }
}
