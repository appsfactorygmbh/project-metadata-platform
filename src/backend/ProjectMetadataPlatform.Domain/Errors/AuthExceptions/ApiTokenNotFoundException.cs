using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.AuthExceptions;

/// <summary>
/// Exception thrown when a ApiToken could not be found.
/// </summary>
public class ApiTokenNotFoundException : EntityNotFoundException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiTokenNotFoundException"/> class with a specified token ID.
    /// </summary>
    /// <param name="tokenId">The ID of the token that was not found.</param>
    public ApiTokenNotFoundException(int tokenId)
        : base("The token with id " + tokenId + " was not found.") { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiTokenNotFoundException"/> class with a specified token name.
    /// </summary>
    /// <param name="tokenName">The name of the token that was not found.</param>
    public ApiTokenNotFoundException(string tokenName)
        : base("The token with name " + tokenName + " was not found.") { }
}
