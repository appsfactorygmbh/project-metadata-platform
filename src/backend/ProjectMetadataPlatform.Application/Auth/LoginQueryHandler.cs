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
    private readonly IRefreshTokenRepository _authRepository;

    private readonly IUsersRepository _userRepository;

    /// <summary>
    /// Creates a new instance of<see cref="LoginQueryHandler" />.
    /// </summary>
    /// <param name="authRepository"></param>
    public LoginQueryHandler(
        IRefreshTokenRepository authRepository,
        IUsersRepository usersRepository
    )
    {
        _authRepository = authRepository;
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
        if (await _authRepository.CheckRefreshTokenExists(request.Email))
        {
            await _authRepository.UpdateRefreshToken(request.Email, refreshToken);
        }
        else
        {
            await _authRepository.StoreRefreshToken(request.Email, refreshToken);
        }
        return new JwtTokens { AccessToken = stringToken, RefreshToken = refreshToken };
    }
}
