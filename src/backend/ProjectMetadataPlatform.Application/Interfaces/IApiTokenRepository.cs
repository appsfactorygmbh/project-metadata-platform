using System.Linq;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Errors.AuthExceptions;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Repository for long lived api and scim tokens.
/// </summary>
public interface IApiTokenRepository
{
    /// <summary>
    /// Gets a List of all Api Tokens.
    /// </summary>
    /// <returns>List of Api Tokens</returns>
    Task<IQueryable<ApiToken>> GetApiTokens();

    /// <summary>
    /// Gets a specific Api Token via its id.
    /// </summary>
    /// <param name="id">Id of the requested Token.</param>
    /// <returns>The requested api token.</returns>
    /// <exception cref="ApiTokenNotFoundException">Thrown if no token exists for the given id.</exception>
    Task<ApiToken> GetApiTokenById(int id);

    /// <summary>
    /// Gets a specific Api Token via its name.
    /// </summary>
    /// <param name="name">Name of the requested Token.</param>
    /// <returns>The requested api token.</returns>
    /// <exception cref="ApiTokenNotFoundException">Thrown if no token exists for the given Name.</exception>
    Task<ApiToken> GetApiTokenByName(string name);

    /// <summary>
    /// Deletes the given Token.
    /// </summary>
    /// <param name="token">token to be deleted.</param>
    /// <returns></returns>
    Task DeleteApiToken(ApiToken token);

    /// <summary>
    /// Checks if a token with the scope scim exists in the database.
    /// </summary>
    /// <returns>boolean representing the existing of a scim token.</returns>
    Task<bool> CheckScimTokenExists();

    /// <summary>
    /// Creates a new Api Token in the database and hashes its token value.
    /// </summary>
    /// <param name="token">Token to be created.</param>
    /// <returns></returns>
    Task StoreApiToken(ApiToken token);

    /// <summary>
    /// Updated a Api Token in the database and hashes its token value.
    /// </summary>
    /// <param name="token">Token to be updated.</param>
    /// <returns></returns>
    Task UpdateApiToken(ApiToken token);

    /// <summary>
    /// Returns the Api Token belonging to the given token string.
    /// </summary>
    /// <param name="token">Token value.</param>
    /// <returns>Verified Token.</returns>
    Task<ApiToken?> GetVerifiedToken(string token);
}
