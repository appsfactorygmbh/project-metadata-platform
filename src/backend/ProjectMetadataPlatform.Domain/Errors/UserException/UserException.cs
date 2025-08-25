using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace ProjectMetadataPlatform.Domain.Errors.UserException;

/// <summary>
/// Represents an abstract base class for user-related exceptions.
/// </summary>
/// <param name="message">The error message that explains the reason for the exception.</param>
public abstract class UserException(string message) : PmpException(message)
{
    /// <summary>
    /// Transforms the specified <see cref="IdentityResult"/> into a readable error message.
    /// </summary>
    protected static string TransformIdentityResult(IdentityResult identityResult)
    {
        var errors = identityResult.Errors.Select(e => e.Description);
        return string.Join(" ", errors);
    }
}
