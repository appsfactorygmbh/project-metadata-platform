using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Handler for the <see cref="RegenerateApiTokenCommand" />.
/// </summary>
public class RegenerateApiTokenCommandHandler : IRequestHandler<RegenerateApiTokenCommand, ApiToken>
{
    private readonly IApiTokenRepository _apiTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogRepository _logRepository;

    /// <summary>
    /// Creates a new instance of <see cref="RegenerateApiTokenCommandHandler" />.
    /// </summary>
    /// <param name="apiTokenRepository"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="logRepository"></param>
    public RegenerateApiTokenCommandHandler(
        IApiTokenRepository apiTokenRepository,
        IUnitOfWork unitOfWork,
        ILogRepository logRepository
    )
    {
        _apiTokenRepository = apiTokenRepository;
        _unitOfWork = unitOfWork;
        _logRepository = logRepository;
    }

    /// <summary>
    /// Creates a new token value and updates the token in the database.
    /// </summary>
    /// <param name="request">Request that is handled.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Updated token with unencrypted value.</returns>
    public async Task<ApiToken> Handle(
        RegenerateApiTokenCommand request,
        CancellationToken cancellationToken
    )
    {
        var apiToken = await _apiTokenRepository.GetApiTokenById(request.TokenId);
        var expirationDate = DateTime.UtcNow.AddYears(1);
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(128));

        apiToken.ExpirationDate = expirationDate;
        apiToken.Token = token;

        await _apiTokenRepository.UpdateApiToken(apiToken);
        await _logRepository.AddApiTokenLogForCurrentActor(
            apiToken,
            Domain.Logs.Action.REGENERATED_API_TOKEN,
            new List<LogChange> { }
        );
        await _unitOfWork.CompleteAsync();
        apiToken.Token = token;
        return apiToken;
    }
}
