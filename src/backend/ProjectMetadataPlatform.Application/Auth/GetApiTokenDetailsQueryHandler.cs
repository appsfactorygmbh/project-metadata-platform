using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Handler for the <see cref="GetApiTokenDetailsQuery" />.
/// </summary>
public class GetApiTokenDetailsQueryHandler
    : IRequestHandler<
        GetApiTokenDetailsQuery,
        (ApiToken, IEnumerable<AuthorizationConstants.Actions>)
    >
{
    private readonly IApiTokenRepository _apiTokenRepository;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of <see cref="GetApiTokenDetailsQueryHandler" />.
    /// </summary>
    /// <param name="apiTokenRepository"></param>
    /// <param name="authorizationService"></param>
    public GetApiTokenDetailsQueryHandler(
        IApiTokenRepository apiTokenRepository,
        IAuthorizationService authorizationService
    )
    {
        _apiTokenRepository = apiTokenRepository;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Returns the requested Token.
    /// </summary>
    /// <param name="request">Request for a token.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The Api Token and allowed actions.</returns>
    public async Task<(ApiToken, IEnumerable<AuthorizationConstants.Actions>)> Handle(
        GetApiTokenDetailsQuery request,
        CancellationToken cancellationToken
    )
    {
        var token = await _apiTokenRepository.GetApiTokenById(request.TokenId);
        if (!await _authorizationService.CheckAccess(token, AuthorizationConstants.Actions.GET))
        {
            throw new UnauthorizedException();
        }
        var permissions = await _authorizationService.GetPermissions(token,[AuthorizationConstants.Actions.EDIT,AuthorizationConstants.Actions.DELETE]);
        return (token, permissions);
    }
}
