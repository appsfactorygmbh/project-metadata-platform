using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Handler for the <see cref="CleanUpRefreshTokensCommand" />.
/// </summary>
public partial class CleanUpRefreshTokensCommandHandler
    : IRequestHandler<CleanUpRefreshTokensCommand>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    private readonly IUnitOfWork _unitOfWork;

    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<CleanUpRefreshTokensCommandHandler> _logger;

    /// <summary>
    /// Creates a new instance of<see cref="CleanUpRefreshTokensCommandHandler" />.
    /// </summary>
    public CleanUpRefreshTokensCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService,
        ILogger<CleanUpRefreshTokensCommandHandler> logger
    )
    {
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
        _logger = logger;
    }

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Deleted {DeletedCount} expired refresh tokens."
    )]
    private static partial void LogTokensDeleted(ILogger logger, int deletedCount);

    /// <summary>
    /// Deletes Expired Refresh Tokens from database
    /// </summary>
    /// <param name="request">Request to be handled.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Handle(
        CleanUpRefreshTokensCommand request,
        CancellationToken cancellationToken = default
    )
    {
        await _authorizationService.BypassAuthorization();
        var tokens = await _refreshTokenRepository.GetExpiredTokens();
        await _refreshTokenRepository.DeleteRefreshTokens(tokens);
        if (_logger.IsEnabled(LogLevel.Information))
        {
            var count = tokens.Count();
            LogTokensDeleted(_logger, count);
        }
        await _unitOfWork.CompleteAsync();
    }
}
