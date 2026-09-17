using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Repository for refresh tokens.
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Saves a refresh Token to the database.
    /// </summary>
    /// <param name="email">associated Email</param>
    /// <param name="refreshToken">Value of the Token</param>
    /// <returns></returns>
    Task StoreRefreshToken(string email, string refreshToken);

    /// <summary>
    /// updates an existing refresh Token.
    /// </summary>
    /// <param name="email">associates Email</param>
    /// <param name="refreshToken">Values of the Token</param>
    /// <returns></returns>
    Task UpdateRefreshToken(string email, string refreshToken);

    /// <summary>
    /// Checks for the existence of a refresh Token for a specific user.
    /// </summary>
    /// <param name="email">Email of a user</param>
    /// <returns>True if a token exists; False if no token exists</returns>
    Task<bool> CheckRefreshTokenExists(string email);

    /// <summary>
    /// Checks if a refresh Token is valid.
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns>true if the token is valid; false if the token isn't valid</returns>
    Task<bool> CheckRefreshTokenRequest(string refreshToken);

    /// <summary>
    /// Gets the email related to a refresh Token.
    /// </summary>
    /// <param name="refreshToken">a refresh Token</param>
    /// <returns>a email</returns>
    Task<string?> GetEmailByRefreshToken(string refreshToken);

    /// <summary>
    /// Gets a List of all refreshToken that have reached their expiration date.
    /// </summary>
    /// <returns>List of tokens</returns>
    Task<IEnumerable<RefreshToken>> GetExpiredTokens();

    /// <summary>
    /// Deletes a List of refreshTokens
    /// </summary>
    /// <param name="refreshTokens">Refresh Tokens to be deleted.</param>
    /// <returns></returns>
    Task DeleteRefreshTokens(IEnumerable<RefreshToken> refreshTokens);
}
