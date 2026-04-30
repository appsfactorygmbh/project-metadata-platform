using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Handler for the <see cref="DeleteApiTokenCommand" />.
/// </summary>
public class DeleteApiTokenCommandHandler : IRequestHandler<DeleteApiTokenCommand>
{
    private readonly IApiTokenRepository _apiTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogRepository _logRepository;

    /// <summary>
    ///  Creates a new instance of <see cref="DeleteApiTokenCommandHandler" />.
    /// </summary>
    /// <param name="apiTokenRepository"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="logRepository"></param>
    public DeleteApiTokenCommandHandler(
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
    /// Deletes a Token from the database.
    /// </summary>
    /// <param name="request">request that is handled.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Handle(DeleteApiTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await _apiTokenRepository.GetApiTokenById(request.TokenId);
        await _apiTokenRepository.DeleteApiToken(token);
        await AddDeleteTokenLog(token);
        await _unitOfWork.CompleteAsync();
    }

    /// <summary>
    /// Creates a new log entry for the token getting deleted.
    /// </summary>
    /// <param name="apiToken">Token that got deleted.</param>
    /// <returns></returns>
    private async Task AddDeleteTokenLog(ApiToken apiToken)
    {
        var changes = new List<LogChange>
        {
            new LogChange
            {
                OldValue = apiToken.Name,
                NewValue = "",
                Property = nameof(apiToken.Name),
            },
        };
        if (apiToken.Scopes != null && apiToken.Scopes.Any())
        {
            changes.Add(
                new LogChange
                {
                    OldValue = String.Join(", ", apiToken.Scopes),
                    NewValue = "",
                    Property = nameof(apiToken.Scopes),
                }
            );
        }

        await _logRepository.AddApiTokenLogForCurrentActor(
            apiToken,
            Domain.Logs.Action.REMOVED_API_TOKEN,
            changes
        );
    }
}
