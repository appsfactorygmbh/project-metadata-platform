using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Errors.AuthExceptions;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Handler for the <see cref="LoginQuery" />
/// </summary>
public class LoginQueryHandler : IRequestHandler<LoginQuery, JwtTokens>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    private readonly IUsersRepository _userRepository;

    /// <summary>
    /// Creates a new instance of<see cref="LoginQueryHandler" />.
    /// </summary>
    /// <param name="refreshTokenRepository"></param>
    /// <param name="usersRepository"></param>
    public LoginQueryHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUsersRepository usersRepository
    )
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = usersRepository;
    }

    /// <summary>
    /// Return the JWT tokens for the given login request.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>JwtTokens when successful.</returns>
    public async Task<JwtTokens> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        if (!_userRepository.CheckLogin(request.Email, request.Password).Result)
        {
            throw new AuthInvalidLoginCredentialsException();
        }
        var stringToken = AccessTokenService.CreateAccessToken(request.Email);
        var refreshToken = Guid.NewGuid().ToString();
        if (await _refreshTokenRepository.CheckRefreshTokenExists(request.Email))
        {
            await _refreshTokenRepository.UpdateRefreshToken(request.Email, refreshToken);
        }
        else
        {
            await _refreshTokenRepository.StoreRefreshToken(request.Email, refreshToken);
        }
        return new JwtTokens { AccessToken = stringToken, RefreshToken = refreshToken };
    }
}
