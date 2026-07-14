using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthExceptions;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Handler for the <see cref="CreateApiTokenCommand" />
/// </summary>
public class CreateApiTokenCommandHandler : IRequestHandler<CreateApiTokenCommand, ApiToken>
{
    private readonly IApiTokenRepository _apiTokenRepository;

    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of<see cref="CreateApiTokenCommandHandler" />.
    /// </summary>
    /// <param name="apiTokenRepository"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="logRepository"></param>
    /// <param name="authorizationService"></param>
    public CreateApiTokenCommandHandler(
        IApiTokenRepository apiTokenRepository,
        IUnitOfWork unitOfWork,
        ILogRepository logRepository,
        IAuthorizationService authorizationService
    )
    {
        _apiTokenRepository = apiTokenRepository;
        _unitOfWork = unitOfWork;
        _logRepository = logRepository;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Creates a new Api Token.
    /// </summary>
    /// <param name="request">Request to be handled.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Newly Created ApiToken with unencrypted value.</returns>
    /// <exception cref="ScimTokenAlreadyExistsException">Thrown if someone tries to create a scim token while there already exists one in the database.</exception>
    public async Task<ApiToken> Handle(
        CreateApiTokenCommand request,
        CancellationToken cancellationToken
    )
    {
        var expirationDate = DateTime.UtcNow.AddYears(1);

        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128));

        var apiToken = new ApiToken
        {
            Name = request.Name,
            Scopes = request.Scopes,
            Token = token,
            ExpirationDate = expirationDate,
        };
        if (
            !await _authorizationService.CheckAccess(
                apiToken,
                AuthorizationConstants.Actions.CREATE
            )
        )
        {
            throw new UnauthorizedException();
        }
        if (
            request.Scopes.Contains(TokenScopes.SCIM)
            && await _apiTokenRepository.CheckScimTokenExists()
        )
        {
            throw new ScimTokenAlreadyExistsException();
        }
        await _apiTokenRepository.StoreApiToken(apiToken);
        await AddCreateTokenLog(apiToken);
        await _unitOfWork.CompleteAsync();

        apiToken.Token = token;
        return apiToken;
    }

    /// <summary>
    /// Adds a log entry to the database for a created token.
    /// </summary>
    /// <param name="apiToken">Newly created Token.</param>
    /// <returns></returns>
    private async Task AddCreateTokenLog(ApiToken apiToken)
    {
        var changes = new List<LogChange>
        {
            new LogChange
            {
                OldValue = "",
                NewValue = apiToken.Name,
                Property = nameof(apiToken.Name),
            },
        };
        if (apiToken.Scopes != null && apiToken.Scopes.Any())
        {
            changes.Add(
                new LogChange
                {
                    OldValue = "",
                    NewValue = String.Join(", ", apiToken.Scopes),
                    Property = nameof(apiToken.Scopes),
                }
            );
        }

        await _logRepository.AddApiTokenLogForCurrentActor(
            apiToken,
            Domain.Logs.Action.ADDED_API_TOKEN,
            changes
        );
    }
}
