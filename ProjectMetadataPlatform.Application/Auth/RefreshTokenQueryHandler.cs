using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Handler for the <see cref="RefreshTokenQuery" />
/// </summary>
public class RefreshTokenQueryHandler : IRequestHandler<RefreshTokenQuery, JwtTokens>
{
    private readonly IAuthRepository _authRepository;

    /// <summary>
    ///     Creates a new instance of<see cref="RefreshTokenQueryHandler" />.
    /// </summary>
    /// <param name="authRepository"></param>
    public RefreshTokenQueryHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="AuthenticationException"></exception>
    public async Task<JwtTokens> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
    {
        if (!await _authRepository.CheckRefreshTokenRequest(request.RefreshToken))
        {
            throw new AuthenticationException("Invalid refresh token.");
        }
        var email = await _authRepository.GetEmailByRefreshToken(request.RefreshToken);
        var stringToken = AccessTokenService.CreateAccessToken(email);
        return new JwtTokens { AccessToken = stringToken, RefreshToken = request.RefreshToken };
    }
}
