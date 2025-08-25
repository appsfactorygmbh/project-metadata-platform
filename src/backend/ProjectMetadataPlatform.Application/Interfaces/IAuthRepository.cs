using System.Threading.Tasks;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Repository for authentication.
/// </summary>
public interface IAuthRepository
{
    /// <summary>
    /// Checks if the given login credentials are correct.
    /// </summary>
    /// <param name="email">Email of the user</param>
    /// <param name="password">Password of the user</param>
    /// <returns>True, if the credentials are correct</returns>
    Task<bool> CheckLogin(string email, string password);

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
}
