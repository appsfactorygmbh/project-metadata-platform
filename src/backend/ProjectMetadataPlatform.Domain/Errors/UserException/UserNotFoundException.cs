using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.UserException;

/// <summary>
/// Exception thrown when a user is not found in the system.
/// </summary>
public class UserNotFoundException : EntityNotFoundException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserNotFoundException"/> class.
    /// </summary>
    /// <param name="userIdentifier">The ID of the user that was not found.</param>
    public UserNotFoundException(string userIdentifier)
        : base($"The user {userIdentifier} was not found.") { }
}
