namespace ProjectMetadataPlatform.Domain.Errors.UserException;

/// <summary>
/// Exception thrown when a user is not authenticated.
/// </summary>
public class UserUnauthenticatedException : UserException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserUnauthenticatedException"/> class.
    /// </summary>
    public UserUnauthenticatedException()
        : base("User not authenticated.") { }
}
