using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Handler for the <see cref="CleanUpRefreshTokensCommand" />.
/// </summary>
public class CleanUpRefreshTokensCommandHandler : IRequestHandler<CleanUpRefreshTokensCommand>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    private readonly IUnitOfWork _unitOfWork;

    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of<see cref="CleanUpRefreshTokensCommandHandler" />.
    /// </summary>
    public CleanUpRefreshTokensCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService
    )
    {
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

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

        await _unitOfWork.CompleteAsync();
    }
}
