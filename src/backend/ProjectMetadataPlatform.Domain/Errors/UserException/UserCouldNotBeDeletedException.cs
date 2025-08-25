using Microsoft.AspNetCore.Identity;

namespace ProjectMetadataPlatform.Domain.Errors.UserException;

/// <summary>
/// Exception thrown when a user could not be deleted.
/// </summary>
public class UserCouldNotBeDeletedException : UserException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserCouldNotBeDeletedException"/> class with a specified user ID and identity result.
    /// </summary>
    /// <param name="userId">The ID of the user that could not be deleted.</param>
    /// <param name="identityResult">The result of the identity operation that caused the exception.</param>
    public UserCouldNotBeDeletedException(string userId, IdentityResult identityResult)
        : base(
            $"User with id {userId} could not be deleted: {TransformIdentityResult(identityResult)}"
        ) { }
}
