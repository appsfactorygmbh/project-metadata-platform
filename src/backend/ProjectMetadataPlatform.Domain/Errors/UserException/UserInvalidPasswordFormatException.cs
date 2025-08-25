using Microsoft.AspNetCore.Identity;

namespace ProjectMetadataPlatform.Domain.Errors.UserException;

/// <summary>
/// Exception thrown when a user provides a password with an invalid format.
/// </summary>
public class UserInvalidPasswordFormatException : UserException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserInvalidPasswordFormatException"/> class with a default error message.
    /// </summary>
    public UserInvalidPasswordFormatException(IdentityResult identityResult)
        : base("Invalid password format: " + TransformIdentityResult(identityResult)) { }
}
