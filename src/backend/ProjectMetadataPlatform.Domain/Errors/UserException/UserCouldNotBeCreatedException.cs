using Microsoft.AspNetCore.Identity;

namespace ProjectMetadataPlatform.Domain.Errors.UserException;

/// <summary>
/// Exception thrown when a user could not be created.
/// </summary>
public class UserCouldNotBeCreatedException : UserException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserCouldNotBeCreatedException"/> class with a specified identity result.
    /// </summary>
    /// <param name="identityResult">The result of the identity operation that caused the exception.</param>
    public UserCouldNotBeCreatedException(IdentityResult identityResult)
        : base("User could not be created: " + TransformIdentityResult(identityResult)) { }
}
